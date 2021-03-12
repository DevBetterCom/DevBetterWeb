using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace DevBetterWeb.Web.Controllers
{
  [Route("create-payment-intent")]
  [ApiController]
  public class PaymentIntentApiController : Controller
  {
    private readonly IPaymentHandlerPrice _paymentHandlerPrice;
    private readonly IPaymentHandlerPaymentIntent _paymentHandlerPaymentIntent;

    public PaymentIntentApiController(IPaymentHandlerPrice paymentHandlerPrice,
      IPaymentHandlerPaymentIntent paymentHandlerPaymentIntent)
    {
      _paymentHandlerPrice = paymentHandlerPrice;
      _paymentHandlerPaymentIntent = paymentHandlerPaymentIntent;
    }

    [HttpPost]
    public ActionResult Create(PaymentIntentCreateRequest request)
    {
      var orderAmount = CalculateOrderAmount(request.SubscriptionPriceId!);
      var paymentIntentId = _paymentHandlerPaymentIntent.CreatePaymentIntent(orderAmount);
      var clientSecret = _paymentHandlerPaymentIntent.GetClientSecret(paymentIntentId);

      return Json(new { clientSecret = clientSecret, id = paymentIntentId });
    }
    private int CalculateOrderAmount(string subscriptionPriceId)
    {

      int price = _paymentHandlerPrice.GetPriceAmount(subscriptionPriceId);
      
      return price;
    }
    public class PaymentIntentCreateRequest
    {
      [JsonProperty("subscriptionpriceid")]
      public string? SubscriptionPriceId { get; set; }
    }
  }
}
