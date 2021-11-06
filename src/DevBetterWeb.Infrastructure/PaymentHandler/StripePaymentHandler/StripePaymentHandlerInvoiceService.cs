using System;
using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;

public class StripePaymentHandlerInvoiceService : IPaymentHandlerInvoice
{
  public string GetBillingReason(string json)
  {
    var stripeEvent = EventUtility.ParseEvent(json);
    var invoice = stripeEvent.Data.Object as Invoice;

    var billingReason = invoice!.BillingReason;

    return billingReason;
  }

  public string GetCustomerId(string json)
  {
    var stripeEvent = EventUtility.ParseEvent(json);
    var invoice = stripeEvent.Data.Object as Invoice;

    var customerId = invoice!.Customer.Id;

    return customerId;
  }

  public decimal GetPaymentAmount(string json)
  {
    var stripeEvent = EventUtility.ParseEvent(json);
    var invoice = stripeEvent.Data.Object as Invoice;

    var amount = (decimal)invoice!.Total;

    return amount;
  }

  public string GetSubscriptionId(string json)
  {
    var stripeEvent = EventUtility.ParseEvent(json);
    var invoice = stripeEvent.Data.Object as Invoice;

    var subscriptionId = invoice!.Subscription.Id;

    return subscriptionId;
  }
}
