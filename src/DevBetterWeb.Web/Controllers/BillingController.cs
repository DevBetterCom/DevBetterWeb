using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;


namespace DevBetterWeb.Web.Controllers
{
  [Route("handle-requires-payment-method")]
  [ApiController]
  public class RequiresPaymentMethodController : Controller
  {
    private readonly PaymentIntentService _paymentIntentService;

    public RequiresPaymentMethodController(PaymentIntentService paymentIntentService)
    {
      _paymentIntentService = paymentIntentService;
    }

    [HttpPost]
    public ActionResult<string> HandleRequiresPaymentMethod(HandleRequiresPaymentMethodRequest req)
    {
      var PaymentIntentId = req.PaymentIntentId;

      var PaymentIntent = _paymentIntentService.Get(PaymentIntentId);

      var message = "Invalid card.";

      if(PaymentIntent.LastPaymentError.DeclineCode != null)
      {
        var declineCode = PaymentIntent.LastPaymentError.DeclineCode;

        switch(declineCode)
        {
          case "generic_decline":
          case "fraudulent":
          case "lost_card":
          case "merchant_blacklist":
          case "stolen_card":
            message = "Your card was declined.";
            break;
          case "expired_card":
            break;
          case "incorrect_cvc":
            break;
          case "processing_error":
            break;
          case "incorrect_number":
            break;
          case "invalid_expiry_month":
            break;
          case "invalid_expiry_year":
            break;
          case "invalid_cvc":
            break;
          default:
            break;
        }
          
      }

      return message;
    }

    public class HandleRequiresPaymentMethodRequest
    {
      [JsonProperty("paymentIntentId")]
      public string? PaymentIntentId { get; set; }

    }
  }

  [Route("create-subscription")]
  [ApiController]
  public class BillingController : Controller
  {
    private readonly PaymentMethodService _paymentMethodService;
    private readonly CustomerService _customerService;
    private readonly SubscriptionService _subscriptionService;

    public BillingController(PaymentMethodService paymentMethodService, CustomerService customerService, SubscriptionService subscriptionService)
    {
      _paymentMethodService = paymentMethodService;
      _customerService = customerService;
      _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public ActionResult<Subscription> CreateSubscription(SubscriptionCreateRequest req)
    {
      var myCustomer = req.CustomerId;
      var myPaymentMethod = req.PaymentMethodId;
      var myPrice = req.PriceId;

      // attach payment method
      var options = new PaymentMethodAttachOptions
      {
        Customer = myCustomer,
      };

      try
      {
        _paymentMethodService.Attach(myPaymentMethod, options);
      }
      catch (StripeException e)
      {
        var error = new SubscriptionError(e.Message);

        return error;
      }

      // update customer's default invoice payment method
      var customerOptions = new CustomerUpdateOptions
      {
        InvoiceSettings = new CustomerInvoiceSettingsOptions
        {
          DefaultPaymentMethod = myPaymentMethod,
        },
      };
      _customerService.Update(myCustomer, customerOptions);

      //create subscription
      var subscriptionOptions = new SubscriptionCreateOptions
      {
        Customer = myCustomer,
        Items = new List<SubscriptionItemOptions>
        {
          new SubscriptionItemOptions
          {
            Price = myPrice,
          },
        },
      };
      subscriptionOptions.AddExpand("latest_invoice.payment_intent");
      try
      {
        Subscription subscription = _subscriptionService.Create(subscriptionOptions);
        return subscription;
      }
      catch (StripeException e)
      {
        var error = new SubscriptionError(e.Message);

        return error;
      }



    }

    internal class SubscriptionError : Subscription
    {
      public string Message { get; set; }

      public SubscriptionError(string message)
      {
        Message = message;
      }

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
