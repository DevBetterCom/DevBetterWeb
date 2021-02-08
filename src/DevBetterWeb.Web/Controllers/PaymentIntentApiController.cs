using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;


namespace DevBetterWeb.Web.Controllers
{
  [Route("create-payment-intent")]
  [ApiController]
  public class PaymentIntentApiController : Controller
  {
    private readonly PriceService _priceService;

    public PaymentIntentApiController(PriceService priceService)
    {
      _priceService = priceService;
    }

    [HttpPost]
    public ActionResult Create(PaymentIntentCreateRequest request)
    {
      var paymentIntents = new PaymentIntentService();
      var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
      {
        Amount = CalculateOrderAmount(request.SubscriptionPriceId!),
        Currency = "usd",
        SetupFutureUsage = "off_session",
      });
      return Json(new { clientSecret = paymentIntent.ClientSecret, id = paymentIntent.Id });
    }
    private int CalculateOrderAmount(string subscriptionTypePriceId)
    {

      var priceObject = _priceService.Get(subscriptionTypePriceId);

      int price = (int)priceObject.UnitAmount!;
      
      return price;
    }
    public class PaymentIntentCreateRequest
    {
      [JsonProperty("subscriptionpriceid")]
      public string? SubscriptionPriceId { get; set; }
    }
  }
}
