using System;
using System.Collections.Generic;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;


namespace DevBetterWeb.Web.Controllers
{
  [Route("create-subscription")]
  [ApiController]
  public class BillingController : Controller
  {
    private readonly IPaymentHandlerPaymentMethod _paymentHandlerPaymentMethod;
    private readonly IPaymentHandlerCustomer _paymentHandlerCustomer;
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;

    public BillingController(IPaymentHandlerPaymentMethod paymentHandlerPaymentMethod,
      IPaymentHandlerCustomer paymentHandlerCustomer, IPaymentHandlerSubscription paymentHandlerSubscription)
    {
      _paymentHandlerPaymentMethod = paymentHandlerPaymentMethod;
      _paymentHandlerCustomer = paymentHandlerCustomer;
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
        var error = new IPaymentHandlerSubscriptionDTO(e.Message);

        return error;
      }

    }

    private IPaymentHandlerSubscriptionDTO CreateSubscription(SubscriptionCreateRequest req)
    {
      var customerId = req.CustomerId;
      var paymentMethodId = req.PaymentMethodId;
      var priceId = req.PriceId;

      // attach payment method
      _paymentHandlerPaymentMethod.AttachPaymentMethodToCustomer(paymentMethodId!, customerId!);

      // update customer's default invoice payment method
      _paymentHandlerCustomer.UpdatePaymentMethod(customerId!, paymentMethodId!);

      //create subscription
      var subscriptionOptions = new SubscriptionCreateOptions
      {
        Customer = customerId,
        Items = new List<SubscriptionItemOptions>
        {
          new SubscriptionItemOptions
          {
            Price = priceId,
          },
        },
      };
      subscriptionOptions.AddExpand("latest_invoice.payment_intent");
      var subscription = _paymentHandlerSubscription.CreateSubscription(customerId!, priceId!);
      return subscription;
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
