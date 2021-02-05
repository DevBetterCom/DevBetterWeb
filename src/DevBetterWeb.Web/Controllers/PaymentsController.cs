using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;


namespace DevBetterWeb.Web.Controllers
{
  public class PaymentsController : Controller
  {
    public readonly IOptions<StripeOptions> options;
    private readonly IStripeClient client;

    public PaymentsController(IOptions<StripeOptions> options)
    {
      this.options = options;
      client = new StripeClient(this.options.Value.stripeSecretKey);
    }

    // Set your secret key. Remember to switch to your live secret key in production!
    // See your keys here: https://dashboard.stripe.com/account/apikeys

    [HttpGet("setup")]
    public SetupResponse Setup()
    {
      return new SetupResponse
      {
        //ProPrice = this.options.Value.ProPrice,
        //BasicPrice = this.options.Value.BasicPrice,
        PublishableKey = this.options.Value.stripePublishableKey,
      };
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
    {
      var options = new SessionCreateOptions
      {
        // See https://stripe.com/docs/api/checkout/sessions/create
        // for additional parameters to pass.
        // {CHECKOUT_SESSION_ID} is a string literal; do not change it!
        // the actual Session ID is returned in the query parameter when your customer
        // is redirected to the success page.
        SuccessUrl = "https://example.com/success.html?session_id={CHECKOUT_SESSION_ID}",
        CancelUrl = "/checkout",
        PaymentMethodTypes = new List<string>
        {
            "card",
        },
        Mode = "subscription",
        LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                Price = req.PriceId,
                // For metered billing, do not pass quantity
                Quantity = 1,
            },
        },
      };
      var service = new SessionService(this.client);
      try
      {
        var session = await service.CreateAsync(options);
        return Ok(new CreateCheckoutSessionResponse
        {
          SessionId = session.Id,
        });
      }
      catch (StripeException e)
      {
        Console.WriteLine(e.StripeError.Message);
        return BadRequest(new ErrorResponse
        {
          ErrorMessage = new ErrorMessage
          {
            Message = e.StripeError.Message,
          }
        });
      }
    }

    [HttpGet("checkout-session")]
    public async Task<IActionResult> CheckoutSession(string sessionId)
    {
      var service = new SessionService(this.client);
      var session = await service.GetAsync(sessionId);
      return Ok(session);
    }

  }

  [Route("create-payment-intent")]
  [ApiController]
  public class PaymentIntentApiController : Controller
  {
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
    private static int CalculateOrderAmount(string subscriptionTypePriceId)
    {
      var service = new PriceService();

      var priceObject = service.Get(subscriptionTypePriceId);

      int price = (int)priceObject.UnitAmount!;
      
      return price;
    }
    public class PaymentIntentCreateRequest
    {
      [JsonProperty("subscriptionpriceid")]
      public string? SubscriptionPriceId { get; set; }
    }
  }

  [Route ("update-payment-intent")]
  [ApiController]
  public class PaymentIntentUpdateController : Controller
  {
    [HttpPost]
    public ActionResult Update(PaymentIntentUpdateRequest request)
    {
      var service = new PaymentIntentService();
            
      service.Update(
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

  [Route ("create-customer")]
  [ApiController]
  public class CustomerCreationController : Controller
  {
    [HttpPost]
    public ActionResult Create(CustomerCreateRequest request)
    {
      var customers = new CustomerService();
      var customer = customers.Create(new CustomerCreateOptions
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

  [Route ("create-subscription")]
  [ApiController]
  public class BillingController: Controller
  {
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
      var service = new PaymentMethodService();
      service.Attach(myPaymentMethod, options);

      // update customer's default invoice payment method
      var customerOptions = new CustomerUpdateOptions
      {
        InvoiceSettings = new CustomerInvoiceSettingsOptions
        {
          DefaultPaymentMethod = myPaymentMethod,
        },
      };
      var customerService = new CustomerService();
      customerService.Update(myCustomer, customerOptions);

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
      var subscriptionService = new SubscriptionService();
      try
      {
        Subscription subscription = subscriptionService.Create(subscriptionOptions);
        return subscription;
      }
      catch (StripeException e)
      {
        Console.WriteLine($"Failed to create subscription.{e}");
        return BadRequest();
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

  [Route("get-email")]
  [ApiController]
  public class EmailGetterController : Controller
  {
    [HttpPost]
    public ActionResult GetEmail(EmailGetRequest request)
    {
      var service = new CustomerService();
      var customer = service.Get(request.CustomerId);

      var email = customer.Email;

      return Json(new { _email = email });
    }

    public class EmailGetRequest
    {
      [JsonProperty("customerId")]
      public string? CustomerId { get; set; }
    }

  }
}
