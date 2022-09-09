using System;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Endpoints;

// TODO: we can use it to create customer by admin
[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
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
