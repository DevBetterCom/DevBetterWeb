using DevBetterWeb.Core.Interfaces;
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

    public string GetSubscriptionId(string json)
    {
      var stripeEvent = EventUtility.ParseEvent(json);
      var subscriptionId = "";

      if (stripeEvent.Type.Contains("subscription"))
      {
        var subscription = stripeEvent.Data.Object as Subscription;

        subscriptionId = subscription!.Id;
      }
      return subscriptionId;
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
  }

}
