using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;


namespace DevBetterWeb.Web.Controllers
{
  [Route ("create-customer")]
  [ApiController]
  public class CustomerCreationController : Controller
  {
    private readonly CustomerService _customerService;

    public CustomerCreationController()
    {
      _customerService = new CustomerService();
    }

    [HttpPost]
    public ActionResult Create(CustomerCreateRequest request)
    {
      var customer = _customerService.Create(new CustomerCreateOptions
      {
        Email = request.Email,
      });

      return Json(new { _customer = customer.Id });
    }

    public class CustomerCreateRequest
    {
      [JsonProperty("email")]
      public string? Email { get; set; }
    }
  }
}
