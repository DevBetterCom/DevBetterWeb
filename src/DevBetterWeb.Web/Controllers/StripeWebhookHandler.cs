using System;
using System.IO;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using DevBetterWeb.Core;


namespace DevBetterWeb.Web.Controllers
{ 
  [Route("api/stripecallback")]
  public class StripeWebhookHandler : Controller
  {
    private readonly ILogger<StripeWebhookHandler> _logger;
    private readonly INewMemberService _newMemberService;
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
    private readonly IPaymentHandlerCustomer _paymentHandlerCustomer;
    private readonly IPaymentHandlerEvent _paymentHandlerEvent;

    private readonly AdminUpdatesWebhook _webhook;

    public StripeWebhookHandler(ILogger<StripeWebhookHandler> logger,
      INewMemberService newMemberService,
      IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer,
      IPaymentHandlerEvent paymentHandlerEvent,
      AdminUpdatesWebhook adminUpdatesWebhook)
    {
      _logger = logger;
      _newMemberService = newMemberService;
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _paymentHandlerCustomer = paymentHandlerCustomer;
      _paymentHandlerEvent = paymentHandlerEvent;
      _webhook = adminUpdatesWebhook;
    }


    [HttpPost]
    public async Task<IActionResult> Index()
    {
      var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

      try
      {
        var stripeEventType = _paymentHandlerEvent.GetEventType(json);

        if (stripeEventType.Equals(StripeConstants.CUSTOMER_SUBSCRIPTION_CREATED_EVENT_TYPE) || 
          stripeEventType.Equals(StripeConstants.CUSTOMER_SUBSCRIPTION_UPDATED_EVENT_TYPE))
        {
          var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
          var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
          var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);
          var status = _paymentHandlerSubscription.GetStatus(subscriptionId);

          if (status.Equals("active"))
          {


            if (string.IsNullOrEmpty(email))
            {
              throw new InvalidEmailException();
            }

            Invitation invite = await _newMemberService.CreateInvitationAsync(email, subscriptionId);

            var webhookStringWithInvite = $"Subscription Id: {subscriptionId}\nCustomer Id: {customerId}\nEmail: {email}\nInvitation: {invite.Id}";
            await _webhook.Send($"Webhook:\n{webhookStringWithInvite}");

            await _newMemberService.SendRegistrationEmailAsync(invite);
          }

        }
        else
        {
          _logger.LogError("Unhandled event type: {0}", stripeEventType);
        }
        return Ok();
      }
      catch (StripeException)
      {
        return BadRequest();
      }
      catch (Exception e)
      {
        _logger.LogError($"{e.GetType()}");
        return BadRequest();
      }
    }
  }
}
