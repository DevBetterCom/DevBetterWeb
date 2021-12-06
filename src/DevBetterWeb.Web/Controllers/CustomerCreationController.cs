using System;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevBetterWeb.Web.Controllers;

[Route("create-customer")]
[ApiController]
public class CustomerCreationController : Controller
{
  private readonly IPaymentHandlerCustomerService _paymentHandlerCustomer;

  public CustomerCreationController(IPaymentHandlerCustomerService paymentHandlerCustomer)
  {
    _paymentHandlerCustomer = paymentHandlerCustomer;
  }

  [HttpPost]
  public ActionResult Create(CustomerCreateRequest request)
  {
    try
    {
      var customerId = _paymentHandlerCustomer.CreateCustomer(request.Email!);

      return Json(new { _customer = customerId });
    }
    catch (Exception e)
    {
      return Json(new { _error = e.Message });
    }
  }

  public class CustomerCreateRequest
  {
    [JsonProperty("email")]
    public string? Email { get; set; }
  }
}
