using System;
using System.Collections.Generic;
using Ardalis.ListStartupServices;
using Autofac;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Services;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Stripe;

namespace DevBetterWeb.Web
{
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
            options.UseSqlServer(Configuration
                .GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME)));
        _isDbContextAdded = true;
      }

      ConfigureServices(services);
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<CookiePolicyOptions>(options =>
      {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AuthMessageSenderOptions"));
      services.Configure<DiscordWebhookUrls>(Configuration.GetSection("DiscordWebhookUrls"));
      services.Configure<StripeOptions>(Configuration.GetSection("StripeOptions"));
      services.Configure<SubscriptionPlanOptions>(Configuration.GetSection("SubscriptionPlanOptions"));

      // TODO: Consider changing to check services collection for dbContext
      // See: https://stackoverflow.com/a/49377724/13729
      if (!_isDbContextAdded)
      {
        string dbName = Guid.NewGuid().ToString();
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration
                .GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME)));
        _isDbContextAdded = true;
      }
      services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

      services.AddAutoMapper(typeof(Startup).Assembly);
      services.AddMediatR(typeof(Startup).Assembly);

      services.AddScoped<IMapCoordinateService, GoogleMapCoordinateService>();

      // configure Stripe
      string stripeApiKey = Configuration.GetSection("StripeOptions").GetSection("stripeSecretKey").Value;
      services.AddStripeServices(stripeApiKey);

      services.AddMemberSubscriptionServices();

      services.AddScoped<IWebhookHandlerService, WebhookHandlerService>();
      services.AddScoped<ICsvService, CsvService>();
      services.AddScoped<IAlumniGraduationService, AlumniGraduationService>();
      services.AddScoped<IGraduationCommunicationsService, GraduationCommunicationsService>();

      services.AddScoped<IUserLookupService, UserLookupService>();
      services.AddScoped<IUserRoleManager, DefaultUserRoleManagerService>();

      // list services
      services.Configure<ServiceConfig>(config =>
      {
        config.Services = new List<ServiceDescriptor>(services);
        config.Path = "/allmyservices";
      });

      services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

      services.AddDailyCheckServices();

      services.AddMvc()
        .AddControllersAsServices()
        .AddRazorRuntimeCompilation();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
      });

      //services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
      string vimeoToken = Configuration[Constants.ConfigKeys.VimeoToken];

      builder.RegisterModule(new DefaultInfrastructureModule(_env.EnvironmentName == "Development", vimeoToken));
    }

    public void Configure(IApplicationBuilder app,
        IWebHostEnvironment env,
        AppDbContext migrationContext)
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

  public static class ConfigureServicesExtensions
  {
    public static void AddStripeServices(this IServiceCollection services, string stripeApiKey)
    {
      StripeConfiguration.ApiKey = stripeApiKey;
      services.AddScoped<IPaymentHandlerSubscription, StripePaymentHandlerSubscriptionService>();
      services.AddScoped<IPaymentHandlerCustomerService, StripePaymentHandlerCustomerService>();
      services.AddScoped<IPaymentHandlerEventService, StripePaymentHandlerEventService>();
      services.AddScoped<IPaymentHandlerPrice, StripePaymentHandlerPriceService>();
      services.AddScoped<IPaymentHandlerPaymentIntent, StripePaymentHandlerPaymentIntentService>();
      services.AddScoped<IPaymentHandlerPaymentMethod, StripePaymentHandlerPaymentMethodService>();
      services.AddScoped<IPaymentHandlerSubscriptionDTO, StripePaymentHandlerSubscriptionDTO>();
      services.AddScoped<IPaymentHandlerSubscriptionCreationService, StripePaymentHandlerSubscriptionCreationService>();
      services.AddScoped<IPaymentHandlerInvoice, StripePaymentHandlerInvoiceService>();
    }

    public static void AddDailyCheckServices(this IServiceCollection services)
    {
      services.AddHostedService<DailyCheckService>();
      services.AddScoped<IDailyCheckPingService, DailyCheckPingService>();
      services.AddScoped<IDailyCheckSubscriptionPlanCountService, DailyCheckSubscriptionPlanCountService>();
    }

    public static void AddMemberSubscriptionServices(this IServiceCollection services)
    {
      services.AddScoped<INewMemberService, NewMemberService>();
      services.AddScoped<IMemberLookupService, MemberLookupService>();
      services.AddScoped<IMemberCancellationService, MemberSubscriptionCancellationService>();
      services.AddScoped<IMemberSubscriptionRenewalService, MemberSubscriptionRenewalService>();
      services.AddScoped<IMemberAddBillingActivityService, MemberAddBillingActivityService>();
      services.AddScoped<IMemberSubscriptionPeriodCalculationsService, MemberSubscriptionPeriodCalculationsService>();
      services.AddScoped<IMemberSubscriptionEndedAdminEmailService, MemberSubscriptionEndedAdminEmailService>();
    }
  }
}
