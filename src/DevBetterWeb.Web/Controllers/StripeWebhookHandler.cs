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

      try
      {
        var stripeEvent = EventUtility.ParseEvent(json);
        var stripeEventType = stripeEvent.Type;

        if (stripeEventType.Equals("customer.subscription.created") || stripeEventType.Equals("customer.subscription.updated"))
        {
          var subscription = stripeEvent.Data.Object as Stripe.Subscription;
          var subscriptionId = subscription!.Id;
          var customerId = subscription.CustomerId;
          var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);
          var status = subscription.Status;

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
          _logger.LogError("Unhandled event type: {0}", stripeEvent.Type);
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
