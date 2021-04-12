using System;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace DevBetterWeb.Web.Controllers
{

  [Route("create-subscription")]
  [ApiController]
  public class BillingController : Controller
  {
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;

    public BillingController(IPaymentHandlerSubscription paymentHandlerSubscription)
    {
      _paymentHandlerSubscription = paymentHandlerSubscription;
    }

    [HttpPost]
    public IPaymentHandlerSubscriptionDTO AttemptToCreateSubscription(SubscriptionCreateRequest req)
    {
      try
      {
        var subscription = CreateSubscription(req);
        return subscription;
      }
      catch (Exception e)
      {
        var error = _paymentHandlerSubscription.CreateSubscriptionError(e.Message);

        return error;
      }

    }

    private IPaymentHandlerSubscriptionDTO CreateSubscription(SubscriptionCreateRequest req)
    {
      var customerId = req.CustomerId;
      var paymentMethodId = req.PaymentMethodId;
      var priceId = req.PriceId;

      //create subscription
      var subscriptionDTO = _paymentHandlerSubscription.CreateSubscription(customerId!, priceId!, paymentMethodId!);
      return subscriptionDTO;
    }

    public class SubscriptionCreateRequest
    {
      [JsonProperty("paymentMethodId")]
      public string? PaymentMethodId { get; set; }

      [JsonProperty("customerId")]
      public string? CustomerId { get; set; }

      [JsonProperty("priceId")]
      public string? PriceId { get; set; }
    }
  }

}
