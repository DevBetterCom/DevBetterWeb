using System;
using System.IO;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Core;


namespace DevBetterWeb.Web.Controllers
{
  [Route(Constants.STRIPE_API_ENDPOINT)]
  public class StripeWebhookHandler : Controller
  {
    private readonly IAppLogger<StripeWebhookHandler> _logger;
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
    private readonly IPaymentHandlerEvent _paymentHandlerEvent;
    private readonly IPaymentHandlerInvoice _paymentHandlerInvoice;

    private readonly IWebhookHandlerService _webhookHandlerService;

    public StripeWebhookHandler(IAppLogger<StripeWebhookHandler> logger,
      IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerEvent paymentHandlerEvent,
      IPaymentHandlerInvoice paymentHandlerInvoice,
      IWebhookHandlerService webhookHandlerService)
    {
      _logger = logger;
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _paymentHandlerEvent = paymentHandlerEvent;
      _paymentHandlerInvoice = paymentHandlerInvoice;
      _webhookHandlerService = webhookHandlerService;
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
          throw new Exception($"Unhandled Stripe event type {stripeEventType}");
        }
        return Ok();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Stripe callback error", json);
        throw;
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
      await _webhookHandlerService.HandleNewCustomerSubscriptionAsync(json);
    }

    private async Task HandleCustomerSubscriptionRenewed(string json)
    {
      await _webhookHandlerService.HandleCustomerSubscriptionRenewedAsync(json);
    }

    private async Task HandleCustomerSubscriptionEnded(string json)
    {
      await _webhookHandlerService.HandleCustomerSubscriptionEndedAsync(json);
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
      await _webhookHandlerService.HandleCustomerSubscriptionCancelledAtPeriodEndAsync(json);
    }
  }
}
