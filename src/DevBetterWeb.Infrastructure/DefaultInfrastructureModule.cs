using Autofac;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Infrastructure.Logging;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace DevBetterWeb.Infrastructure
{
  public class DefaultInfrastructureModule : Module
  {
    private bool _isDevelopment = false;
    public DefaultInfrastructureModule(bool isDevelopment)
    {
      _isDevelopment = isDevelopment;
    }

    protected override void Load(ContainerBuilder builder)
    {
      if (_isDevelopment)
      {
        RegisterDevelopmentOnlyDependencies(builder);
      }
      else
      {
        RegisterProductionOnlyDependencies(builder);
      }
      RegisterCommonDependencies(builder);
      RegisterPaymentHandlerDependencies(builder);
    }

    private void RegisterCommonDependencies(ContainerBuilder builder)
    {
      builder.RegisterType<DomainEventDispatcher>().InstancePerLifetimeScope();
      builder.RegisterType<DomainEventDispatcher>().As<IDomainEventDispatcher>();
      builder.RegisterType<MemberRegistrationService>().As<IMemberRegistrationService>();
      builder.RegisterType<DefaultEmailSender>().As<IEmailSender>();
      builder.RegisterType<AspNetCoreIdentityUserRoleMembershipService>()
          .As<IUserRoleMembershipService>();
      builder.RegisterType<AspNetCoreIdentityUserEmailConfirmationService>()
          .As<IUserEmailConfirmationService>();
      builder.RegisterType<AdminUpdatesWebhook>().InstancePerDependency();
      builder.RegisterType<BookDiscussionWebhook>().InstancePerDependency();
      builder.RegisterType<DevBetterComNotificationsWebhook>().InstancePerDependency();
      builder.RegisterGeneric(typeof(LoggerAdapter<>))
        .As(typeof(IAppLogger<>))
        .InstancePerDependency();

      builder.RegisterDecorator<LoggerEmailServiceDecorator, IEmailService>();

      builder.RegisterAssemblyTypes(this.ThisAssembly)
          .AsClosedTypesOf(typeof(IHandle<>));
    }

    private void RegisterPaymentHandlerDependencies(ContainerBuilder builder)
    {
      builder.RegisterType<CustomerService>();
      builder.RegisterType<PaymentMethodService>();
      builder.RegisterType<SubscriptionService>();
      builder.RegisterType<PriceService>();
      builder.RegisterType<PaymentIntentService>();
      builder.RegisterType<PaymentMethodService>();
    }

    private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
    {
      builder.RegisterType<LocalSmtpEmailService>().As<IEmailService>();
    }

    private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
    {
      builder.RegisterType<SendGridEmailService>().As<IEmailService>();

    }

  }
}
