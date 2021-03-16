using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace DevBetterWeb.Web.Controllers
{
  [Route ("create-customer")]
  [ApiController]
  public class CustomerCreationController : Controller
  {
    private readonly IPaymentHandlerCustomer _paymentHandlerCustomer;

    public CustomerCreationController(IPaymentHandlerCustomer paymentHandlerCustomer)
    {
      _paymentHandlerCustomer = paymentHandlerCustomer;
    }

    [HttpPost]
    public ActionResult Create(CustomerCreateRequest request)
    {
      var customerId = _paymentHandlerCustomer.CreateCustomer(request.Email!);

      return Json(new { _customer = customerId });
    }

    public class CustomerCreateRequest
    {
      [JsonProperty("email")]
      public string? Email { get; set; }
    }
  }
}
