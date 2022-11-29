using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.ListStartupServices;
using Autofac;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using DevBetterWeb.Web.Interfaces;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DevBetterWeb.Web;

public class Startup
{
  private bool _isDbContextAdded = false;
  private readonly IWebHostEnvironment _env;

  public Startup(IConfiguration config, IWebHostEnvironment env)
  {
    Configuration = config;
    _env = env;
  }

  public IConfiguration Configuration { get; }
  public ILifetimeScope? AutofacContainer { get; private set; }

  public void ConfigureProductionServices(IServiceCollection services)
  {
    if (!_isDbContextAdded)
    {
      services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(Configuration!
              .GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME)));
      _isDbContextAdded = true;
    }

    // configure Stripe
    string stripeApiKey = Configuration!
      .GetSection("StripeOptions")!
      .GetSection("StripeSecretKey")!.Value!;
    services.AddStripeServices(stripeApiKey);

    services.AddDailyCheckServices();
    services.AddStartupNotificationService();

    ConfigureServices(services);
  }

  public void ConfigureTestingServices(IServiceCollection services)
  {
    string dbName = Guid.NewGuid().ToString();

    services.AddDbContext<AppDbContext>(options =>
      options.UseInMemoryDatabase(dbName));

    ConfigureServices(services);
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.Configure<CookiePolicyOptions>(options =>
    {
      options.CheckConsentNeeded = context => true;
      options.MinimumSameSitePolicy = SameSiteMode.None;
    });

    services.AddLogging();

    services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AuthMessageSenderOptions"));
    services.Configure<DiscordWebhookUrls>(Configuration.GetSection("DiscordWebhookUrls"));
    services.Configure<StripeOptions>(Configuration.GetSection("StripeOptions"));
    services.Configure<SubscriptionPlanOptions>(Configuration.GetSection("SubscriptionPlanOptions"));
    services.Configure<ApiSettings>(Configuration.GetSection("ApiSettings"));

    // TODO: Consider changing to check services collection for dbContext
    // See: https://stackoverflow.com/a/49377724/13729

    if (!services.Any(x => x.ServiceType == typeof(AppDbContext)))
    {
      services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(Configuration
              .GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME)));
    }
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

    var webProjectAssembly = typeof(Startup).Assembly;
    services.AddAutoMapper(webProjectAssembly);

    services.AddScoped<IMapCoordinateService, GoogleMapCoordinateService>();
    services.AddScoped<IJsonParserService, JsonParserService>();

    services.AddMemberSubscriptionServices();

    services.AddScoped<IWebhookHandlerService, WebhookHandlerService>();
    services.AddScoped<ICsvService, CsvService>();
    services.AddScoped<IAlumniGraduationService, AlumniGraduationService>();
    services.AddScoped<IGraduationCommunicationsService, GraduationCommunicationsService>();

    services.AddScoped<IUserLookupService, UserLookupService>();
    services.AddScoped<IUserRoleManager, DefaultUserRoleManagerService>();

    services.AddScoped<IVideosService, VideosService>();
    services.AddSingleton<IVideosCacheService, VideosCacheService>();

    services.AddScoped<IWebVTTParsingService, WebVTTParsingService>();
    services.AddScoped<IVideoDetailsService, VideoDetailsService>();
    
    // list services
    services.Configure<ServiceConfig>(config =>
    {
      config.Services = new List<ServiceDescriptor>(services);
      config.Path = "/allmyservices";
    });

    services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

    services.AddMvc()
      .AddControllersAsServices()
      .AddRazorRuntimeCompilation();

    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    });

		services.AddApplicationInsightsTelemetry(options =>
		{
			options.ConnectionString = Configuration["APPINSIGHTS_CONNECTIONSTRING"];
		});
//    services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
  }

  public void ConfigureContainer(ContainerBuilder builder)
  {
    string vimeoToken = Configuration![Constants.ConfigKeys.VimeoToken]!;
    builder.RegisterModule(new DefaultInfrastructureModule(_env.EnvironmentName == "Development", vimeoToken));
  }

  public void Configure(IApplicationBuilder app,
      IWebHostEnvironment env)
  {
    if (env.EnvironmentName == "Development")
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

    if (env.EnvironmentName == "Development")
    {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });
    }

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapRazorPages();
      endpoints.MapDefaultControllerRoute();
    });
  }
}
