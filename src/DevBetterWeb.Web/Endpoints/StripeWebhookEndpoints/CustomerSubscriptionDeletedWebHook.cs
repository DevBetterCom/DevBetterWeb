using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.PaymentHandler;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace DevBetterWeb.Web.Endpoints;

public class CustomerSubscriptionDeletedWebHook : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult
{
	private readonly IWebhookHandlerService _webHookHandlerService;
	private readonly string _stripeWebHookSecretKey;
	public CustomerSubscriptionDeletedWebHook(
		IOptions<StripeOptions> optionsAccessor,
		IWebhookHandlerService webHookHandlerService)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.Null(optionsAccessor.Value?.StripeWebHookSecretKey, nameof(optionsAccessor.Value.StripeWebHookSecretKey));

		_webHookHandlerService = webHookHandlerService;
		_stripeWebHookSecretKey = optionsAccessor.Value.StripeWebHookSecretKey;
	}

	[HttpPost("stripe-customer-subscription-deleted-web-hook")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeWebHookSecretKey);

			if (stripeEvent.Type != Events.CustomerDeleted)
			{
				throw new Exception($"Unhandled Stripe event type {stripeEvent.Type}");
			}

			await HandleCustomerSubscriptionEnded(json);

			return Ok();
		}
		catch (StripeException exception)
		{
			return BadRequest(exception.Message);
		}
	}

	private Task HandleCustomerSubscriptionEnded(string json)
	{
		return _webHookHandlerService.HandleCustomerSubscriptionEndedAsync(json);
	}
}
