using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace DevBetterWeb.Web;

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
    services.AddScoped<IVideosThumbnailService, VideosThumbnailService>();
  }

  public static void AddStartupNotificationService(this IServiceCollection services)
  {
    services.AddHostedService<StartupNotificationService>();
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
