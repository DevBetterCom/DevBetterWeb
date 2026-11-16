using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ListStartupServices;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Modules.Testimonials;
using DevBetterWeb.Web;
using DevBetterWeb.Web.Areas.Identity;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using NimblePros.Metronome;
using Serilog;

// 29 Aug 2023 - Getting a nullref in here somewhere maybe? Also a stack overflow during startup somewhere.

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

Console.WriteLine($"Startup ENV: {builder.Environment.EnvironmentName}");
var isProduction = builder.Environment.IsEnvironment("Production");
bool isDevelopment = builder.Environment.IsDevelopment();
var isTesting = builder.Environment.IsEnvironment("Testing");

// When running under Aspire (an OTLP endpoint is configured), forward Serilog events to the
// registered logger providers so the OpenTelemetry provider exports them to the dashboard.
// Serilog already writes to the console/file/SQL sinks, so clear the built-in providers first
// (Serilog bypasses provider filters when forwarding) and let AddServiceDefaults add OpenTelemetry.
bool forwardLogsToProviders = builder.IsOtlpExporterEnabled();
if (forwardLogsToProviders)
{
	builder.Logging.ClearProviders();
}

// Aspire service defaults: OpenTelemetry, health checks, resilience, service discovery
builder.AddServiceDefaults();

if (!isTesting)
{
	builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration),
		writeToProviders: forwardLogsToProviders);
}
else
{
	builder.Host.UseSerilog((_, config) => config.MinimumLevel.Information().WriteTo.Console(),
		writeToProviders: forwardLogsToProviders);
}
builder.Services.Configure<CookiePolicyOptions>(options =>
{
	options.CheckConsentNeeded = context => true;
	options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddLogging();

builder.Services.Configure<ApiMailSenderOptions>(builder.Configuration.GetSection("ApiMailSenderOptions")); 
builder.Services.Configure<DiscordWebhookUrls>(builder.Configuration.GetSection("DiscordWebhookUrls"));
builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("StripeOptions"));
builder.Services.Configure<SubscriptionPlanOptions>(builder.Configuration.GetSection("SubscriptionPlanOptions"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));


if (isProduction)
{
	builder.Services.AddDbContext<AppDbContext>((provider, options) =>
		options.UseSqlServer(builder.Configuration
			.GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME))
					.AddMetronomeDbTracking(provider)
		);

	// Other prod-only services
	builder.Services.AddStartupNotificationService();
}
else if (!isTesting)
{
	// Fallback for other non-test, non-prod envs (e.g., Development, Staging)
	builder.Services.AddDbContext<AppDbContext>((provider, options) =>
		options.UseSqlServer(builder.Configuration
			.GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME))
					.AddMetronomeDbTracking(provider)
		);

}

// Testing hosts register their own (in-memory) provider for the module's DbContext.
builder.Services.AddTestimonialsModule(isTesting
	? null
	: (provider, options) => options
		.UseTestimonialsSqlServer(builder.Configuration.GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME))
		.AddMetronomeDbTracking(provider));

builder.Services.AddInfrastructureServices(isDevelopment);

builder.Services.AddMediator(options =>
{
	// Handlers (e.g. DomainEventHandler<T>) depend on scoped services such as repositories and
	// DbContexts, so they must not be singletons (Mediator's default lifetime).
	options.ServiceLifetime = ServiceLifetime.Scoped;
	options.Assemblies =
	[
		typeof(IAggregateRoot),
		typeof(AppDbContext),
		typeof(Program)
	];
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddDailyCheckServices(isProduction);
builder.Services.AddStripeServices(
	builder.Configuration.GetSection("StripeOptions")["StripeSecretKey"]!);

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));

builder.Services.AddMetronome();

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
builder.Services.AddScoped<IAddCreatedVideoToFolderService, AddCreatedVideoToFolderService>();
builder.Services.AddScoped<ICreateVideoService, CreateVideoService>();
builder.Services.AddScoped(typeof(ILocalMigrationService<>), typeof(MigrationService<>));

VimeoSettings vimeoSettings = builder.Configuration.GetSection(Constants.ConfigKeys.VimeoSettings)!.Get<VimeoSettings>()!;
builder.Services.AddSingleton(vimeoSettings);
builder.Services.AddDevBetterVimeoServices(vimeoSettings.Token);

// list services
builder.Services.Configure<ServiceConfig>(config =>
{
	config.Services = new List<ServiceDescriptor>(builder.Services);
	config.Path = "/allmyservices";
});

builder.Services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

builder.Services.AddMvc()
	.AddControllersAsServices();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var appInsightsConnectionString = builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"];
if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
{
	builder.Services.AddApplicationInsightsTelemetry(options =>
	{
		options.ConnectionString = appInsightsConnectionString;
	});
}


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseShowAllServicesMiddleware();
	app.UseMetronomeLoggingMiddleware();
}
else
{
	app.Urls.Add($"http://*:{port}");
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

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

app.MapDefaultEndpoints();

app.MapRazorPages();

app.UseStaticFiles();
app.MapDefaultControllerRoute();

// seed database
await ApplyLocalMigrationsAsync(app);
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

async Task ApplyLocalMigrationsAsync(WebApplication webApplication)
{
	using var scope = webApplication.Services.CreateScope();

	var identity = scope.ServiceProvider.GetRequiredService<ILocalMigrationService<IdentityDbContext>>();

	var app = scope.ServiceProvider.GetRequiredService<ILocalMigrationService<AppDbContext>>();

	var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
	bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var runningInContainer);
	// Set by the Aspire AppHost so a fresh local SQL Server container gets migrated on startup.
	bool applyLocalMigrations = webApplication.Configuration.GetValue<bool>("ApplyLocalMigrations");
	bool shouldApplyMigrations = MigrationServiceExtensions.ShouldApplyLocalMigrations(runningInContainer, applyLocalMigrations);

	await identity.ApplyLocalMigrationAsync(environment, shouldApplyMigrations);
	await app.ApplyLocalMigrationAsync(environment, shouldApplyMigrations);

	if (environment == "Development" && shouldApplyMigrations)
	{
		await scope.ServiceProvider.ApplyTestimonialsMigrationsAsync();
	}
}

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program
{
}
