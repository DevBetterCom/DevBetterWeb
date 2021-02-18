using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<PaymentsController> _logger;
    private readonly SessionService _sessionService;

    public PaymentsController(IOptions<StripeOptions> options, ILogger<PaymentsController> logger, SessionService sessionService)
    {
      this.options = options;
      client = new StripeClient(this.options.Value.stripeSecretKey);
      _logger = logger;
      _sessionService = sessionService;
    }

    [HttpGet("setup")]
    public SetupResponse Setup()
    {
      return new SetupResponse
      {
        PublishableKey = options.Value.stripePublishableKey,
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
      try
      {
        var session = await _sessionService.CreateAsync(options);
        return Ok(new CreateCheckoutSessionResponse
        {
          SessionId = session.Id,
        });
      }
      catch (StripeException e)
      {
        _logger.LogInformation(e.StripeError.Message);
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
      var session = await _sessionService.GetAsync(sessionId);
      return Ok(session);
    }

  }
}

