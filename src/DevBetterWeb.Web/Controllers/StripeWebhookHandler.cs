using System;
using System.IO;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
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

    private readonly AdminUpdatesWebhook _webhook;

    public StripeWebhookHandler(ILogger<StripeWebhookHandler> logger,
      INewMemberService newMemberService,
      IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer,
      AdminUpdatesWebhook adminUpdatesWebhook)
    {
      _logger = logger;
      _newMemberService = newMemberService;
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _paymentHandlerCustomer = paymentHandlerCustomer;
      _webhook = adminUpdatesWebhook;
    }


    [HttpPost]
    public async Task<IActionResult> Index()
    {
      var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
      _webhook.Content = $"{json}";
      await _webhook.Send();

      return Ok();

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

          Invitation invite = await _newMemberService.CreateInvitationAsync(email, subscriptionId);

          await _newMemberService.SendRegistrationEmailAsync(invite);
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
      catch (Exception)
      {
        return BadRequest();
      }
    }
  }

  //Test Purposes
  //  [HttpPost]
  //  public async Task<IActionResult> Index()
  //  {
  //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

  //    try
  //    {
  //      var stripeEvent = EventUtility.ParseEvent(json);
  //      var stripeEventType = stripeEvent.Type;

  //      if (stripeEventType.Equals("customer.subscription.created"))
  //      {

  //        var subscription = stripeEvent.Data.Object as Stripe.Subscription;
  //        var subscriptionId = subscription!.Id;
  //        var customerId = subscription.CustomerId;
  //        var email = "mycustomer@test.com";

  //        Invitation invite = await _newMemberService.CreateInvitation(email, subscriptionId);

  //        //await _newMemberService.SendRegistrationEmail(invite);
  //      }
  //      else
  //      {
  //        _logger.LogError("Unhandled event type: {0}", stripeEvent.Type);
  //      }
  //      return Ok();
  //    }
  //    catch (StripeException)
  //    {
  //      return BadRequest();
  //    }
  //    catch (Exception)
  //    {
  //      return BadRequest();
  //    }
  //  }
  //}

}
