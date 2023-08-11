﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ListStartupServices;
using Autofac.Extensions.DependencyInjection;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web;
using DevBetterWeb.Web.Areas.Identity;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using GoogleReCaptcha.V3.Interface;
using GoogleReCaptcha.V3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Autofac;
using DevBetterWeb.Infrastructure;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
	options.CheckConsentNeeded = context => true;
	options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddLogging();

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("AuthMessageSenderOptions"));
builder.Services.Configure<DiscordWebhookUrls>(builder.Configuration.GetSection("DiscordWebhookUrls"));
builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("StripeOptions"));
builder.Services.Configure<SubscriptionPlanOptions>(builder.Configuration.GetSection("SubscriptionPlanOptions"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));


// PRODUCTION SERVICES
if (builder.Environment.EnvironmentName.ToLower() == "production")
{
	builder.Services.AddDbContext<AppDbContext>(options =>
			options.UseSqlServer(builder.Configuration!
					.GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME)));

	// configure Stripe
	string stripeApiKey = builder.Configuration!
		.GetSection("StripeOptions")!
		.GetSection("StripeSecretKey")!.Value!;
	builder.Services.AddStripeServices(stripeApiKey);

	builder.Services.AddDailyCheckServices();
	builder.Services.AddStartupNotificationService();
}

// TEST SERVICES
//if (builder.Environment.EnvironmentName.ToLower() == "testing")
//{
//	string dbName = Guid.NewGuid().ToString();

//	builder.Services.AddDbContext<AppDbContext>(options =>
//		options.UseInMemoryDatabase(dbName));
//}

if (!builder.Services.Any(x => x.ServiceType == typeof(AppDbContext)))
{
	builder.Services.AddDbContext<AppDbContext>(options =>
			options.UseSqlServer(builder.Configuration
					.GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME)));
}
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

var webProjectAssembly = typeof(Program).Assembly;
builder.Services.AddAutoMapper(webProjectAssembly);

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<BackgroundTaskService>();

builder.Services.AddScoped<IMapCoordinateService, GoogleMapCoordinateService>();
builder.Services.AddScoped<IJsonParserService, JsonParserService>();

builder.Services.AddMemberSubscriptionServices();

builder.Services.AddScoped<IWebhookHandlerService, WebhookHandlerService>();
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<IAlumniGraduationService, AlumniGraduationService>();
builder.Services.AddScoped<IGraduationCommunicationsService, GraduationCommunicationsService>();

builder.Services.AddScoped<IUserLookupService, UserLookupService>();
builder.Services.AddScoped<IUserRoleManager, DefaultUserRoleManagerService>();

builder.Services.AddScoped<IVideosService, VideosService>();
builder.Services.AddSingleton<IVideosCacheService, VideosCacheService>();

builder.Services.AddScoped<IWebVTTParsingService, WebVTTParsingService>();
builder.Services.AddScoped<IVideoDetailsService, VideoDetailsService>();
builder.Services.AddScoped<IFilteredLeaderboardService, FilteredLeaderboardService>();
builder.Services.AddScoped<IBookCategoryService, BookCategoryService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IRankAndOrderService, RankAndOrderService>();
builder.Services.AddScoped<IRankingService, RankingService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IFilteredBookDetailsService, FilteredBookDetailsService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

// list services
builder.Services.Configure<ServiceConfig>(config =>
{
	config.Services = new List<ServiceDescriptor>(builder.Services);
	config.Path = "/allmyservices";
});

builder.Services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

builder.Services.AddMvc()
	.AddControllersAsServices()
	.AddRazorRuntimeCompilation();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddApplicationInsightsTelemetry(options =>
{
	options.ConnectionString = builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"];
});

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
	//	containerBuilder.RegisterModule(new DefaultCoreModule());
	string vimeoToken = builder.Configuration![Constants.ConfigKeys.VimeoToken]!;
	bool isDevelopment = builder.Environment.EnvironmentName == "Development";
	containerBuilder.RegisterModule(new DefaultInfrastructureModule(isDevelopment,
		vimeoToken));
});

// TODO: Configure Testing and Production Services from Startup

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseShowAllServicesMiddleware();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
	});
}

app.MapRazorPages();
app.MapDefaultControllerRoute();

// seed database
await SeedDatabase(app);

app.Run();


static async Task SeedDatabase(IHost host)
{
	using var scope = host.Services.CreateScope();
	var services = scope.ServiceProvider;
	var logger = services.GetRequiredService<ILogger<Program>>();
	var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
	logger.LogInformation($"Current environment: {environment}");

	var context = services.GetRequiredService<AppDbContext>();
	var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
	SeedData.PopulateInitData(context, userManager);

	//TODO: need to be checked without token.
	var videosCacheService = services.GetRequiredService<IVideosCacheService>();
	await videosCacheService.UpdateAllVideosAsync();

	if (environment == "Production")
	{
		return;
	}

	logger.LogInformation("Seeding database...");
	try
	{
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		if (userManager.Users.Any() || roleManager.Roles.Any())
		{
			logger.LogDebug("User/Role data already exists.");
		}
		else
		{
			await AppIdentityDbContextSeed.SeedAsync(userManager, roleManager);
			logger.LogDebug("Populated AppIdentityDbContext test data.");
		}
		//var context = services.GetRequiredService<AppDbContext>();
		if (await context.Questions!.AnyAsync())
		{
			logger.LogDebug("Database already has data in it.");
		}
		else
		{
			SeedData.PopulateTestData(context, userManager);
			logger.LogDebug("Populated AppDbContext test data.");
		}
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
	finally
	{
		logger.LogInformation("Finished seeding database...");
	}
}

//static IHostBuilder CreateHostBuilder(string[] args) =>
//		Host.CreateDefaultBuilder(args)
//			.UseSerilog()
//			.UseServiceProviderFactory(new AutofacServiceProviderFactory())
//			.ConfigureWebHostDefaults(webBuilder =>
//			{
//				webBuilder.ConfigureKestrel(serverOptions =>
//						{
//							serverOptions.Limits.MaxRequestBodySize = Constants.MAX_UPLOAD_FILE_SIZE; // 500MB
//						})
//						.UseStartup<Startup>()
//						.ConfigureLogging(logging =>
//						{
//							logging.AddAzureWebAppDiagnostics();
//						});
//			});


// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program
{
}
