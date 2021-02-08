using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;


namespace DevBetterWeb.Web.Controllers
{
  [Route ("create-subscription")]
  [ApiController]
  public class BillingController: Controller
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
      _paymentMethodService.Attach(myPaymentMethod, options);

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
        return BadRequest(e);
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
