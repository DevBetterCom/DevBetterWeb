using System;
using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripeSubscription : IPaymentHandlerSubscription
  {

    public DateTime GetEndDate(string subscriptionId)
    {
      var subscription = GetSubscription(subscriptionId);

      DateTime endDate = subscription.CurrentPeriodEnd;

      return endDate;
    }

    public DateTime GetStartDate(string subscriptionId)
    {
      var subscription = GetSubscription(subscriptionId);

      DateTime startDate = subscription.CurrentPeriodStart;

      return startDate;
    }

    private Stripe.Subscription GetSubscription(string subscriptionId)
    {
      var subscriptionService = new SubscriptionService();

      var subscription = subscriptionService.Get(subscriptionId);

      return subscription;
    }

  }

}
