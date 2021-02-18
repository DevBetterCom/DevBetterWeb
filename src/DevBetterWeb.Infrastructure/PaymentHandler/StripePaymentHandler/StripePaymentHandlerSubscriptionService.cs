using System;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.ValueObjects;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerSubscriptionService : IPaymentHandlerSubscription
  {
    private readonly SubscriptionService _subscriptionService;

    public StripePaymentHandlerSubscriptionService(SubscriptionService subscriptionService)
    {
      _subscriptionService = subscriptionService;
    }

    public DateTimeRange GetDateTimeRange(string subscriptionId)
    {
      var subscription = GetSubscription(subscriptionId);

      var startDate = GetStartDate(subscription);
      var endDate = GetEndDate(subscription);

      var dateTimeRange = new DateTimeRange(startDate, endDate);

      return dateTimeRange;
    }

    private DateTime GetEndDate(Subscription subscription)
    {
      DateTime endDate = subscription.CurrentPeriodEnd;

      return endDate;
    }

    private DateTime GetStartDate(Subscription subscription)
    {
      DateTime startDate = subscription.CurrentPeriodStart;

      return startDate;
    }

    private Stripe.Subscription GetSubscription(string subscriptionId)
    {

      var subscription = _subscriptionService.Get(subscriptionId);

      return subscription;
    }

  }

}
