using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace DevBetterWeb.Web.Endpoints;

public class InvoicePaidWebHook : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult
{
	private readonly IWebhookHandlerService _webHookHandlerService;
	private readonly IPaymentHandlerInvoice _paymentHandlerInvoice;
	private readonly string _stripeWebHookSecretKey;
	public InvoicePaidWebHook(
		IOptions<StripeOptions> optionsAccessor,
		IWebhookHandlerService webHookHandlerService,
		IPaymentHandlerInvoice paymentHandlerInvoice)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.Null(optionsAccessor.Value?.StripeWebHookSecretKey, nameof(optionsAccessor.Value.StripeWebHookSecretKey));

		_webHookHandlerService = webHookHandlerService;
		_paymentHandlerInvoice = paymentHandlerInvoice;
		_stripeWebHookSecretKey = optionsAccessor.Value.StripeWebHookSecretKey;
	}

	[HttpPost("stripe-invoice-paid-web-hook")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeWebHookSecretKey);

			// Was InvoicePaymentSucceeded changed to InvoicePaid
			if (stripeEvent.Type != Events.InvoicePaid)
			{
				throw new Exception($"Unhandled Stripe event type {stripeEvent.Type}");
			}

			await HandleInvoicePaymentSucceeded(json);

			return Ok();
		}
		catch (StripeException exception)
		{
			return BadRequest(exception.Message);
		}
	}

	private async Task HandleInvoicePaymentSucceeded(string json)
	{
		var billingReason = _paymentHandlerInvoice.GetBillingReason(json);

		if (billingReason == StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_CREATION)
		{
			await HandleNewCustomerSubscription(json);
		}
		else if (billingReason == StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_RENEWAL)
		{
			await HandleCustomerSubscriptionRenewed(json);
		}
	}

	private Task HandleNewCustomerSubscription(string json)
	{
		return _webHookHandlerService.HandleNewCustomerSubscriptionAsync(json);
	}

	private Task HandleCustomerSubscriptionRenewed(string json)
	{
		return _webHookHandlerService.HandleCustomerSubscriptionRenewedAsync(json);
	}
}
