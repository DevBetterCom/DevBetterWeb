﻿using Autofac;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.InvoiceHandler.StripeInvoiceHandler;
using DevBetterWeb.Infrastructure.IssuingHandler.StripeIssuingHandler;
using DevBetterWeb.Infrastructure.Logging;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using NimblePros.Vimeo.Extensions;
using Stripe;
using Stripe.Issuing;
using CardService = Stripe.Issuing.CardService;

namespace DevBetterWeb.Infrastructure;

public class DefaultInfrastructureModule : Module
{
  private bool _isDevelopment = false;
  private string _vimeoToken = string.Empty;
  public DefaultInfrastructureModule(bool isDevelopment, string vimeoToken)
  {
    _isDevelopment = isDevelopment;
    _vimeoToken = vimeoToken;
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
    RegisterIssuingHandlerDependencies(builder);

		AutofacExtensions.RegisterVimeoServicesDependencies(builder, _vimeoToken);
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
    builder.RegisterType<DiscordWebhookService>().As<IDiscordWebhookService>();
		builder.RegisterType<AdminUpdatesWebhook>().InstancePerDependency();
    builder.RegisterType<CoachingSessionsWebhook>().InstancePerDependency();
    builder.RegisterType<BookDiscussionWebhook>().InstancePerDependency();
    builder.RegisterType<DevBetterComNotificationsWebhook>().InstancePerDependency();
    builder.RegisterGeneric(typeof(LoggerAdapter<>))
      .As(typeof(IAppLogger<>))
      .InstancePerDependency();

    builder.RegisterDecorator<LoggerEmailServiceDecorator, IEmailService>();
    builder.RegisterType<MarkdigService>()
      .As<IMarkdownService>();

    builder.RegisterAssemblyTypes(this.ThisAssembly)
        .AsClosedTypesOf(typeof(IHandle<>));

    builder.RegisterType<NonCurrentMembersService>().As<INonCurrentMembersService>();
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

  private void RegisterIssuingHandlerDependencies(ContainerBuilder builder)
  {
	  builder.RegisterType<TransactionService>();
	  builder.RegisterType<CardService>();
	  builder.RegisterType<InvoiceService>();
	  builder.RegisterType<StripeIssuingHandlerCardListService>().As<IIssuingHandlerCardListService>();
	  builder.RegisterType<StripeIssuingHandlerTransactionListService>().As<IIssuingHandlerTransactionListService>();
	  builder.RegisterType<StripeInvoiceHandlerListService>().As<IInvoiceHandlerListService>();
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
