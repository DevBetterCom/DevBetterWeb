using System;
using System.IO;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Core;


namespace DevBetterWeb.Web.Controllers
{

  [Route(Constants.STRIPE_API_ENDPOINT)]
  public class StripeWebhookHandler : Controller
  {
    private readonly ILogger<StripeWebhookHandler> _logger;
    private readonly INewMemberService _newMemberService;
    private readonly IMemberCancellationService _memberCancellationService;
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
    private readonly IPaymentHandlerCustomer _paymentHandlerCustomer;
    private readonly IPaymentHandlerEvent _paymentHandlerEvent;
    private readonly IPaymentHandlerInvoice _paymentHandlerInvoice;
    private readonly IMemberAddBillingActivityService _memberAddBillingActivityService;
    private readonly IMemberSubscriptionRenewalService _memberSubscriptionRenewalService;

    private readonly AdminUpdatesWebhook _webhook;

    public StripeWebhookHandler(ILogger<StripeWebhookHandler> logger,
      INewMemberService newMemberService,
      IMemberCancellationService memberCancellationService,
      IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer,
      IPaymentHandlerEvent paymentHandlerEvent,
      IPaymentHandlerInvoice paymentHandlerInvoice,
      IMemberAddBillingActivityService memberAddBillingActivityService,
      IMemberSubscriptionRenewalService memberSubscriptionRenewalService,
      AdminUpdatesWebhook adminUpdatesWebhook)
    {
      _logger = logger;
      _newMemberService = newMemberService;
      _memberCancellationService = memberCancellationService;
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _paymentHandlerCustomer = paymentHandlerCustomer;
      _paymentHandlerEvent = paymentHandlerEvent;
      _paymentHandlerInvoice = paymentHandlerInvoice;
      _memberAddBillingActivityService = memberAddBillingActivityService;
      _memberSubscriptionRenewalService = memberSubscriptionRenewalService;
      _webhook = adminUpdatesWebhook;
    }


    [HttpPost]
    public async Task<IActionResult> Index()
    {
      var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

      try
      {
        var stripeEventType = _paymentHandlerEvent.GetEventType(json);

        if (stripeEventType.Equals(StripeConstants.INVOICE_PAYMENT_SUCCEEDED_EVENT_TYPE))
        {
          await HandleInvoicePaymentSucceeded(json);
        }
        else if (stripeEventType.Equals(StripeConstants.CUSTOMER_SUBSCRIPTION_DELETED_EVENT_TYPE))
        {
          await HandleCustomerSubscriptionEnded(json);
        }
        else if (stripeEventType.Equals(StripeConstants.CUSTOMER_SUBSCRIPTION_UPDATED_EVENT_TYPE))
        {
          await HandleCustomerSubscriptionUpdatedEvent(json);
        }
        else
        {
          _logger.LogError("Unhandled event type: {0}", stripeEventType);
        }
        return Ok();
      }
      catch (Exception e)
      {
        _logger.LogError($"{e.GetType()}");
        return BadRequest();
      }
    }

    private async Task HandleInvoicePaymentSucceeded(string json)
    {
      var billingReason = _paymentHandlerInvoice.GetBillingReason(json);

      if (billingReason == StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_CREATION)
      {
        await HandleNewCustomerSubscription(json);
      }
      else if (billingReason == StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_RENEWAL)
      {
        await HandleCustomerSubscriptionRenewed(json);
      }
    }

    private async Task HandleNewCustomerSubscription(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var status = _paymentHandlerSubscription.GetStatus(subscriptionId);

      if (status == "active")
      {
        var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
        var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

        if (string.IsNullOrEmpty(email))
        {
          throw new InvalidEmailException();
        }

        Invitation invite = await _newMemberService.CreateInvitationAsync(email, subscriptionId);

        var webhookMessage = $"A new customer with email {email} has subscribed to DevBetter. They will be receiving a registration email.";
        await _webhook.Send($"Webhook:\n{webhookMessage}");

        await _newMemberService.SendRegistrationEmailAsync(invite);

        var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

        await _memberAddBillingActivityService.AddSubscriptionCreationBillingActivity(email, paymentAmount);
      }
    }

    private async Task HandleCustomerSubscriptionRenewed(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      var subscriptionDates = _paymentHandlerSubscription.GetDateTimeRange(subscriptionId);
      var endDate = (DateTime) subscriptionDates.EndDate;

      await _memberSubscriptionRenewalService.ExtendMemberSubscription(email, endDate);

      var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

      await _memberAddBillingActivityService.AddSubscriptionRenewalBillingActivity(email, paymentAmount);
    }

    private async Task HandleCustomerSubscriptionEnded(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      await _memberCancellationService.RemoveUserFromMemberRoleAsync(email);
      await _memberCancellationService.SendCancellationEmailAsync(email);
      await _memberAddBillingActivityService.AddSubscriptionEndingBillingActivity(email);
    }

    private async Task HandleCustomerSubscriptionUpdatedEvent(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var cancelAtPeriodEnd = _paymentHandlerSubscription.GetCancelAtPeriodEnd(subscriptionId);

      if (cancelAtPeriodEnd)
      {
        await HandleCustomerSubscriptionCancelledAtPeriodEnd(json);
      }
    }

    private async Task HandleCustomerSubscriptionCancelledAtPeriodEnd(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      await _memberCancellationService.SendFutureCancellationEmailAsync(email);
      await _memberAddBillingActivityService.AddSubscriptionCancellationBillingActivity(email);
    }


  }
}
