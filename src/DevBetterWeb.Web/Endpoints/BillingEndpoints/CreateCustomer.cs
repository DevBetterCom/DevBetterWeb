using System;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Endpoints;

public class CreateCustomer : EndpointBaseSync
	.WithRequest<CustomerCreateRequest>
	.WithResult<CustomerCreateResponse>
{
	private readonly IPaymentHandlerCustomerService _paymentHandlerCustomer;

	public CreateCustomer(IPaymentHandlerCustomerService paymentHandlerCustomer)
	{
		_paymentHandlerCustomer = paymentHandlerCustomer;
	}

	[HttpPost("create-customer")]
	public override CustomerCreateResponse Handle([FromBody] CustomerCreateRequest request)
	{
		try
		{
			var customer = _paymentHandlerCustomer.CreateCustomer(request.Email!);

			return new CustomerCreateResponse(customer.CustomerId, customer.Email);
		}
		catch (Exception e)
		{
			return new CustomerCreateResponse(e.Message);
		}
	}
}
