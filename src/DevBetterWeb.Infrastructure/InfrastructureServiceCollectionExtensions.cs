using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.InvoiceHandler.StripeInvoiceHandler;
using DevBetterWeb.Infrastructure.IssuingHandler.StripeIssuingHandler;
using DevBetterWeb.Infrastructure.Logging;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Stripe.Issuing;
using CardService = Stripe.Issuing.CardService;

namespace DevBetterWeb.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, bool isDevelopment, string vimeoToken)
	{
		if (isDevelopment)
		{
			services.AddTransient<IEmailService, LocalSmtpEmailService>();
		}
		else
		{
			services.AddTransient<IEmailService, Smtp2GoEmailService>();
		}

		// Common Dependencies
		services.AddScoped<IDomainEventDispatcher,MediatRDomainEventDispatcher>();
//		services.AddScoped<IDomainEventDispatcher>(sp => sp.GetRequiredService<DomainEventDispatcher>());

		services.AddScoped<IMemberRegistrationService, MemberRegistrationService>();
		services.AddScoped<IEmailSender, DefaultEmailSender>();
		services.AddScoped<IUserRoleMembershipService, AspNetCoreIdentityUserRoleMembershipService>();
		services.AddScoped<IUserEmailConfirmationService, AspNetCoreIdentityUserEmailConfirmationService>();
		services.AddScoped<IDiscordWebhookService, DiscordWebhookService>();

		services.AddTransient<AdminUpdatesWebhook>();
		services.AddTransient<CoachingSessionsWebhook>();
		services.AddTransient<BookDiscussionWebhook>();
		services.AddTransient<DevBetterComNotificationsWebhook>();

		services.AddTransient(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

		// services.Decorate<IEmailService, LoggerEmailServiceDecorator>(); // Uncomment if using Scrutor

		services.AddScoped<IMarkdownService, MarkdigService>();

		// Domain Event Handlers
		var assembliesToScan = new[] {
			typeof(IAggregateRoot).Assembly, // Core
			typeof(AppDbContext).Assembly, // Infrastructure
		};
		services.AddDomainEventHandlers(assembliesToScan);

		services.AddScoped<INonCurrentMembersService, NonCurrentMembersService>();

		// Stripe Payment Services
		services.AddTransient<CustomerService>();
		services.AddTransient<PaymentMethodService>();
		services.AddTransient<SubscriptionService>();
		services.AddTransient<PriceService>();
		services.AddTransient<PaymentIntentService>();

		// Stripe Issuing Services
		services.AddTransient<TransactionService>();
		services.AddTransient<CardService>();
		services.AddTransient<InvoiceService>();

		services.AddTransient<IIssuingHandlerCardListService, StripeIssuingHandlerCardListService>();
		services.AddTransient<IIssuingHandlerTransactionListService, StripeIssuingHandlerTransactionListService>();
		services.AddTransient<IInvoiceHandlerListService, StripeInvoiceHandlerListService>();

		// Vimeo
		services.RegisterVimeoServicesDependencies(vimeoToken);

		return services;
	}
}
