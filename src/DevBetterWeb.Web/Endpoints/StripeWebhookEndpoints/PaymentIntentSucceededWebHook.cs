using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace DevBetterWeb.Web.Endpoints;

public class PaymentIntentSucceededWebHook : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult
{
	private readonly string _stripeWebHookSecretKey;
	private readonly IAppLogger<InvoicePaidWebHook> _logger;

	public PaymentIntentSucceededWebHook(
		IAppLogger<InvoicePaidWebHook> logger, 
		IOptions<StripeOptions> optionsAccessor)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.Null(optionsAccessor.Value?.StripePaymentIntentSucceededWebHookSecretKey, nameof(optionsAccessor.Value.StripePaymentIntentSucceededWebHookSecretKey));

		_stripeWebHookSecretKey = optionsAccessor.Value.StripePaymentIntentSucceededWebHookSecretKey;
		_logger = logger;
	}

	[HttpPost("stripe-payment-intent-succeeded-web-hook")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Start Stripe Endpoint: stripe-payment-intent-succeeded-web-hook");

		var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

		try
		{
			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeWebHookSecretKey);

			_logger.LogInformation($"Processing Stripe Event Type: {stripeEvent.Type}");

			if (stripeEvent.Type != Events.PaymentIntentSucceeded)
			{
				throw new Exception($"Unhandled Stripe event type {stripeEvent.Type}");
			}

			var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

			return Ok();
		}
		catch (StripeException exception)
		{
			_logger.LogError(exception, "Stripe callback error", json);
			return BadRequest(exception.Message);
		}
	}
}
