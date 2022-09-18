using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace DevBetterWeb.Web.Endpoints;

public class CustomerSubscriptionDeletedWebHook : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult
{
	private readonly IAppLogger<CustomerSubscriptionDeletedWebHook> _logger;
	private readonly IWebhookHandlerService _webHookHandlerService;
	private readonly string _stripeWebHookSecretKey;
	public CustomerSubscriptionDeletedWebHook(
		IAppLogger<CustomerSubscriptionDeletedWebHook> logger,
		IOptions<StripeOptions> optionsAccessor,
		IWebhookHandlerService webHookHandlerService)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.Null(optionsAccessor.Value?.StripeWebHookSecretKey, nameof(optionsAccessor.Value.StripeWebHookSecretKey));

		_logger = logger;
		_webHookHandlerService = webHookHandlerService;
		_stripeWebHookSecretKey = optionsAccessor.Value.StripeWebHookSecretKey;
	}

	[HttpPost("stripe-customer-subscription-deleted-web-hook")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Start Stripe Endpoint: stripe-customer-subscription-deleted-web-hook");

		var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

		try
		{
			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeWebHookSecretKey);

			_logger.LogInformation($"Processing Stripe Event Type: {stripeEvent.Type}");

			if (stripeEvent.Type != Events.CustomerDeleted)
			{
				throw new Exception($"Unhandled Stripe event type {stripeEvent.Type}");
			}

			await HandleCustomerSubscriptionEnded(json);

			return Ok();
		}
		catch (StripeException exception)
		{
			_logger.LogError(exception, "Stripe callback error", json);
			return BadRequest(exception.Message);
		}
	}

	private Task HandleCustomerSubscriptionEnded(string json)
	{
		return _webHookHandlerService.HandleCustomerSubscriptionEndedAsync(json);
	}
}
