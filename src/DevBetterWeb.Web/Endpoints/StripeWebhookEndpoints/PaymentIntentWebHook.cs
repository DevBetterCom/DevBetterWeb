using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace DevBetterWeb.Web.Endpoints;

public class PaymentIntentWebHook : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult
{
	private readonly string _stripeWebHookSecretKey;
	private readonly IPaymentHandlerCustomerService _paymentHandlerCustomer;
	public PaymentIntentWebHook(IOptions<StripeOptions> optionsAccessor, IPaymentHandlerCustomerService paymentHandlerCustomer)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.Null(optionsAccessor.Value?.StripeWebHookSecretKey, nameof(optionsAccessor.Value.StripeWebHookSecretKey));

		_stripeWebHookSecretKey = optionsAccessor.Value.StripeWebHookSecretKey;
		_paymentHandlerCustomer = paymentHandlerCustomer;
	}

	[HttpPost("stripe-payment-intent-web-hook")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeWebHookSecretKey);

			switch (stripeEvent.Type)
			{
				case Events.PaymentIntentAmountCapturableUpdated:
					break;
				case Events.PaymentIntentCanceled:
					break;
				case Events.PaymentIntentCreated:
					break;
				case Events.PaymentIntentPartiallyFunded:
					break;
				case Events.PaymentIntentPaymentFailed:
					break;
				case Events.PaymentIntentProcessing:
					break;
				case Events.PaymentIntentRequiresAction:
					break;
				case Events.PaymentIntentSucceeded:
				{
					var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
					var email = paymentIntent.Charges.Data.FirstOrDefault()?.BillingDetails.Email;
					if (string.IsNullOrEmpty(email))
					{
						email = paymentIntent.Customer.Email;
					}
					if (!string.IsNullOrEmpty(email))
					{
						_ = _paymentHandlerCustomer.CreateCustomer(email);
					}
				
					break;
				}
			}

			return Ok();
		}
		catch (StripeException exception)
		{
			return BadRequest(exception.Message);
		}
	}
}
