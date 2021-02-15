using System;
using System.IO;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;


namespace DevBetterWeb.Web.Controllers
{
  [Route("api/stripecallback")]
  public class StripeWebhookHandler : Controller
  {
    private readonly ILogger<StripeWebhookHandler> _logger;
    private readonly INewMemberService _newMemberService;
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
    private readonly IPaymentHandlerCustomer _paymentHandlerCustomer;

    public StripeWebhookHandler(ILogger<StripeWebhookHandler> logger,
      INewMemberService newMemberService,
      IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer)
    {
      _logger = logger;
      _newMemberService = newMemberService;
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _paymentHandlerCustomer = paymentHandlerCustomer;
    }


    [HttpPost]
    public async Task<IActionResult> Index()
    {
      var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

      try
      {
        var stripeEvent = EventUtility.ParseEvent(json);
        var stripeEventType = stripeEvent.Type;

        if (stripeEventType.Equals("customer.subscription.created"))
        {

          var subscription = stripeEvent.Data.Object as Stripe.Subscription;
          var subscriptionId = subscription!.Id;
          var customerId = subscription.CustomerId;
          var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

          Invitation invite = await _newMemberService.CreateInvitation(email, subscriptionId);

          await _newMemberService.SendRegistrationEmail(invite);
        }
        else
        {
          _logger.LogError("Unhandled event type: {0}", stripeEvent.Type);
        }
        return Ok();
      }
      catch (StripeException)
      {
        return BadRequest();
      }
      catch(Exception)
      {
        return BadRequest();
      }
    }
  }

}
