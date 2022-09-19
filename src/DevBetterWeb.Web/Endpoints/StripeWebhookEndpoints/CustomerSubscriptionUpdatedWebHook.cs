using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace DevBetterWeb.Web.Endpoints;

public class CustomerSubscriptionUpdatedWebHook : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult
{
	private readonly IAppLogger<CustomerSubscriptionDeletedWebHook> _logger;
	private readonly IPaymentHandlerEventService _paymentHandlerEventService;
	private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
	private readonly IWebhookHandlerService _webHookHandlerService;
	private readonly string _stripeWebHookSecretKey;
	public CustomerSubscriptionUpdatedWebHook(
		IAppLogger<CustomerSubscriptionDeletedWebHook> logger,
		IOptions<StripeOptions> optionsAccessor, 
		IPaymentHandlerEventService paymentHandlerEventService,
		IPaymentHandlerSubscription paymentHandlerSubscription,
		IWebhookHandlerService webHookHandlerService)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.Null(optionsAccessor.Value?.StripeCustomerSubscriptionUpdatedWebHookSecretKey, nameof(optionsAccessor.Value.StripeCustomerSubscriptionUpdatedWebHookSecretKey));

		_logger = logger;
		_paymentHandlerEventService = paymentHandlerEventService;
		_paymentHandlerSubscription = paymentHandlerSubscription;
		_webHookHandlerService = webHookHandlerService;
		_stripeWebHookSecretKey = optionsAccessor.Value.StripeCustomerSubscriptionUpdatedWebHookSecretKey;
	}

	[HttpPost("stripe-customer-subscription-updated-web-hook")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Start Stripe Endpoint: stripe-customer-subscription-updated-web-hook");

		string json;
		using (var streamReader = new StreamReader(HttpContext.Request.Body))
		{
			json = await streamReader.ReadToEndAsync();
		}

		try
		{
			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeWebHookSecretKey);

			_logger.LogInformation($"Processing Stripe Event Type: {stripeEvent.Type}");

			if (stripeEvent.Type != Events.CustomerUpdated)
			{
				throw new Exception($"Unhandled Stripe event type {stripeEvent.Type}");
			}

			var paymentHandlerEvent = _paymentHandlerEventService.FromJson(json);
			var cancelAtPeriodEnd = _paymentHandlerSubscription.GetCancelAtPeriodEnd(paymentHandlerEvent.SubscriptionId);

			if (cancelAtPeriodEnd)
			{
				await HandleCustomerSubscriptionCancelledAtPeriodEnd(json);
			}

			return Ok();
		}
		catch (StripeException exception)
		{
			_logger.LogError(exception, "Stripe callback error", json);
			return BadRequest(exception.Message);
		}
	}

	private Task HandleCustomerSubscriptionCancelledAtPeriodEnd(string json)
	{
		return _webHookHandlerService.HandleCustomerSubscriptionCancelledAtPeriodEndAsync(json);
	}
}
