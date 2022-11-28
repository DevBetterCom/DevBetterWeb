using System;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Endpoints;

public class CreateSubscription : EndpointBaseSync
	.WithRequest<SubscriptionCreateRequest>
	.WithResult<SubscriptionCreateResponse>
{
	private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
	private readonly IPaymentHandlerCustomerService _paymentHandlerCustomer;
	private readonly IPaymentHandlerPaymentMethod _paymentHandlerPaymentMethod;

	public CreateSubscription(
		IPaymentHandlerSubscription paymentHandlerSubscription, 
		IPaymentHandlerCustomerService paymentHandlerCustomer,
		IPaymentHandlerPaymentMethod paymentHandlerPaymentMethod)
	{
		_paymentHandlerSubscription = paymentHandlerSubscription;
		_paymentHandlerCustomer = paymentHandlerCustomer;
		_paymentHandlerPaymentMethod = paymentHandlerPaymentMethod;
	}

	[HttpPost("create-subscription")]
	public override SubscriptionCreateResponse Handle([FromBody] SubscriptionCreateRequest request)
	{
		try
		{
			var customer = _paymentHandlerCustomer.CreateCustomer(request.CustomerEmail!);
			var subscription = _paymentHandlerSubscription.CreateSubscription(customer.CustomerId, request.PriceId!, request.PaymentMethodId!);
			var result = new SubscriptionCreateResponse(subscription._id!, subscription._status!, subscription._latestInvoicePaymentIntentStatus!, subscription._latestInvoicePaymentIntentClientSecret!, subscription._errorMessage);

			return result;
		}
		catch (Exception exception)
		{
			return new SubscriptionCreateResponse(exception.Message);
		}
	}
}
