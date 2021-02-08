using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;


namespace DevBetterWeb.Web.Controllers
{
  [Route ("update-payment-intent")]
  [ApiController]
  public class PaymentIntentUpdateController : Controller
  {
    private readonly PaymentIntentService _paymentIntentService;

    public PaymentIntentUpdateController(PaymentIntentService paymentIntentService)
    {
      _paymentIntentService = paymentIntentService;
    }

    [HttpPost]
    public ActionResult Update(PaymentIntentUpdateRequest request)
    {
          
      _paymentIntentService.Update(
        request.PaymentIntentSecret,
        new PaymentIntentUpdateOptions
        {
          Customer = request.Customer,
        });

      return Json(new { clientSecret = request.PaymentIntentSecret });
    }

    public class PaymentIntentUpdateRequest
    {
      [JsonProperty("customer")]
      public string? Customer { get; set; }
      [JsonProperty("paymentIntentSecret")]
      public string? PaymentIntentSecret { get; set; }

    }
    }
}
