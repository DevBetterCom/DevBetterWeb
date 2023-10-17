using System;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Endpoints;

// TODO: we can use it later so it is disabled now.
[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class CreatePaymentIntent : EndpointBaseSync
	.WithRequest<PaymentIntentCreateRequest>
	.WithActionResult<PaymentIntentCreateResponse>
{
	private readonly IPaymentHandlerPrice _paymentHandlerPrice;
	private readonly IPaymentHandlerPaymentIntent _paymentHandlerPaymentIntent;

	public CreatePaymentIntent(IPaymentHandlerPrice paymentHandlerPrice, IPaymentHandlerPaymentIntent paymentHandlerPaymentIntent)
	{
		_paymentHandlerPrice = paymentHandlerPrice;
		_paymentHandlerPaymentIntent = paymentHandlerPaymentIntent;
	}

	[HttpPost("create-payment-intent")]
	public override ActionResult<PaymentIntentCreateResponse> Handle([FromBody] PaymentIntentCreateRequest request)
	{
		try
		{
			var orderAmount = _paymentHandlerPrice.GetPriceAmount(request.SubscriptionPriceId!);
			var paymentIntentId = _paymentHandlerPaymentIntent.CreatePaymentIntent(orderAmount);
			var clientSecret = _paymentHandlerPaymentIntent.GetClientSecret(paymentIntentId);

			return new PaymentIntentCreateResponse(paymentIntentId, clientSecret);
		}
		catch (Exception e)
		{
			return BadRequest(new PaymentIntentCreateResponse(e.Message)); 
		}
	}
}
