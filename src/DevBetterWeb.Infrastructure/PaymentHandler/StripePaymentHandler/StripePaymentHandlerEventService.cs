using System;
using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerEventService : IPaymentHandlerEvent
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
  }

}
