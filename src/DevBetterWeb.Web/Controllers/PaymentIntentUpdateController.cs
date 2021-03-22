using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevBetterWeb.Web.Controllers
{
  [Route ("update-payment-intent")]
  [ApiController]
  public class PaymentIntentUpdateController : Controller
  {
    private readonly IPaymentHandlerPaymentIntent _paymentHandlerPaymentIntent;

    public PaymentIntentUpdateController(IPaymentHandlerPaymentIntent paymentHandlerPaymentIntent)
    {
      _paymentHandlerPaymentIntent = paymentHandlerPaymentIntent;
    }

    [HttpPost]
    public ActionResult Update(PaymentIntentUpdateRequest request)
    {
      _paymentHandlerPaymentIntent.UpdatePaymentIntent(request.PaymentIntentSecret!, request.Customer!);

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
