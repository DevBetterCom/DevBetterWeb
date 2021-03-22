using System;
using System.Collections.Generic;
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

    public IPaymentHandlerSubscriptionDTO CreateSubscription(string customerId, string priceId)
    {
      var subscriptionOptions = new SubscriptionCreateOptions
      {
        Customer = customerId,
        Items = new List<SubscriptionItemOptions>
        {
          new SubscriptionItemOptions
          {
            Price = priceId,
          },
        },
      };
      subscriptionOptions.AddExpand("latest_invoice.payment_intent");

      var subscription = _subscriptionService.Create(subscriptionOptions);

      var id = subscription.Id;
      var status = subscription.Status;
      var latestInvoicePaymentIntentStatus = subscription.LatestInvoice.PaymentIntent.Status;
      var latestInvoicePaymentIntentClientSecret = subscription.LatestInvoice.PaymentIntent.ClientSecret;

      var subscriptionDTO = new StripePaymentHandlerSubscriptionDTO(id, status, latestInvoicePaymentIntentStatus, latestInvoicePaymentIntentClientSecret);

      return subscriptionDTO;
    }

    public IPaymentHandlerSubscriptionDTO CreateSubscriptionError(string errorMessage)
    {
      var subscriptionError = new StripePaymentHandlerSubscriptionDTO(errorMessage);
      return subscriptionError;
    }

    public string GetCustomerId(string subscriptionId)
    {
      var subscription = GetSubscription(subscriptionId);

      var customerId = subscription.CustomerId;

      return customerId; 
    }

    public DateTimeRange GetDateTimeRange(string subscriptionId)
    {
      var subscription = GetSubscription(subscriptionId);

      var startDate = GetStartDate(subscription);
      var endDate = GetEndDate(subscription);

      var dateTimeRange = new DateTimeRange(startDate, endDate);

      return dateTimeRange;
    }

    public string GetStatus(string subscriptionId)
    {
      var subscription = GetSubscription(subscriptionId);

      var status = subscription.Status;

      return status;
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
