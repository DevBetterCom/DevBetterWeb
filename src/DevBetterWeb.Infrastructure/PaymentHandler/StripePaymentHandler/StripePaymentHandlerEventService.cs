using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerEventService : IPaymentHandlerEventService
  {
    public string GetEventType(string json)
    {
      var stripeEvent = EventUtility.ParseEvent(json);
      var stripeEventType = stripeEvent.Type;

      return stripeEventType;
    }

    // TODO: Add tests
    public string GetSubscriptionId(string json)
    {
      var stripeEvent = EventUtility.ParseEvent(json);

      var invoice = stripeEvent.Data.Object as Invoice;
      if(invoice != null)
      {
        return invoice.SubscriptionId;
      }

      var subscription = stripeEvent.Data.Object as Subscription;
      if(subscription != null)
      {
        return subscription.Id;
      }

      return string.Empty;
    }

    private string GetSubscriptionId(Stripe.Event stripeEvent)
    {
      var invoice = stripeEvent.Data.Object as Invoice;
      if (invoice != null)
      {
        return invoice.SubscriptionId;
      }

      var subscription = stripeEvent.Data.Object as Subscription;
      if (subscription != null)
      {
        return subscription.Id;
      }

      return string.Empty;

    }

    public string GetInvoiceId(string json)
    {
      var stripeEvent = EventUtility.ParseEvent(json);
      var invoiceId = "";

      if (stripeEvent.Type.StartsWith("invoice"))
      {
        var invoice = stripeEvent.Data.Object as Invoice;

        invoiceId = invoice!.Id;
      }
      return invoiceId;
    }

    private string GetInvoiceId(Stripe.Event stripeEvent)
    {
      var invoiceId = "";

      if (stripeEvent.Type.StartsWith("invoice"))
      {
        var invoice = stripeEvent.Data.Object as Invoice;

        invoiceId = invoice!.Id;
      }
      return invoiceId;
    }

    public PaymentHandlerEvent FromJson(string json)
    {
      var stripeEvent = EventUtility.ParseEvent(json);
      string stripeEventType = stripeEvent.Type;
      string invoiceId = GetInvoiceId(stripeEvent);
      string subscriptionId = GetSubscriptionId(stripeEvent);

      return new PaymentHandlerEvent(stripeEventType, invoiceId, subscriptionId);
    }
  }
}
