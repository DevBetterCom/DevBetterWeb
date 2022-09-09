using System;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Endpoints;

// TODO: we can use it to create customer by admin
[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
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
			// var paymentMethodId = _paymentHandlerPaymentMethod.PaymentMethodCreateByCard(request.CustomerEmail!, request.CardNumber!, request.CardExpMonth, request.CardExpYear, request.CardCvc!);
			var subscription = _paymentHandlerSubscription.CreateSubscription(customer.CustomerId, request.PriceId!, request.PaymentMethodId!);
			var result = new SubscriptionCreateResponse(subscription._id, subscription._status, subscription._latestInvoicePaymentIntentStatus, subscription._errorMessage);

			return result;
		}
		catch (Exception exception)
		{
			return new SubscriptionCreateResponse(exception.Message);
		}
	}
}
