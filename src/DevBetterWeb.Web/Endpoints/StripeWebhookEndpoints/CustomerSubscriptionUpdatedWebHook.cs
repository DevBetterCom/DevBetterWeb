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
	private readonly IPaymentHandlerEventService _paymentHandlerEventService;
	private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
	private readonly IWebhookHandlerService _webHookHandlerService;
	private readonly string _stripeWebHookSecretKey;
	public CustomerSubscriptionUpdatedWebHook(
		IOptions<StripeOptions> optionsAccessor, 
		IPaymentHandlerEventService paymentHandlerEventService,
		IPaymentHandlerSubscription paymentHandlerSubscription,
		IWebhookHandlerService webHookHandlerService)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.Null(optionsAccessor.Value?.StripeWebHookSecretKey, nameof(optionsAccessor.Value.StripeWebHookSecretKey));

		_paymentHandlerEventService = paymentHandlerEventService;
		_paymentHandlerSubscription = paymentHandlerSubscription;
		_webHookHandlerService = webHookHandlerService;
		_stripeWebHookSecretKey = optionsAccessor.Value.StripeWebHookSecretKey;
	}

	[HttpPost("stripe-customer-subscription-updated-web-hook")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeWebHookSecretKey);

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
			return BadRequest(exception.Message);
		}
	}

	private Task HandleCustomerSubscriptionCancelledAtPeriodEnd(string json)
	{
		return _webHookHandlerService.HandleCustomerSubscriptionCancelledAtPeriodEndAsync(json);
	}
}
