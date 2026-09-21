using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.ValueObjects;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;

public class StripePaymentHandlerInvoiceService : IPaymentHandlerInvoice
{
  private readonly InvoiceService _invoiceService;

  public StripePaymentHandlerInvoiceService(InvoiceService invoiceService)
  {
    _invoiceService = invoiceService;
  }

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

    var subscriptionId = invoice!.Parent?.SubscriptionDetails?.SubscriptionId ?? string.Empty;

    return subscriptionId;
  }

  public PaidInvoiceDetails GetInvoiceDetails(string invoiceId)
  {
    var invoice = _invoiceService.Get(invoiceId);

    return new PaidInvoiceDetails(
      invoice.Id,
      invoice.Parent?.SubscriptionDetails?.SubscriptionId ?? string.Empty,
      invoice.BillingReason,
      invoice.Status,
      invoice.Total);
  }
}
