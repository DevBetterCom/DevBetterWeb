using System.Threading.Tasks;
using DevBetterWeb.Core.ValueObjects;
using System;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerSubscription
  {
    DateTimeRange GetDateTimeRange(string subscriptionId);
    DateTime GetEndDate(string subscriptionId);
    string GetStatus(string subscriptionId);
    string GetCustomerId(string subscriptionId);
    IPaymentHandlerSubscriptionDTO CreateSubscription(string customerId, string priceId, string paymentMethodId);
    IPaymentHandlerSubscriptionDTO CreateSubscriptionError(string errorMessage);
    bool GetCancelAtPeriodEnd(string subscriptionId);
    Task CancelSubscriptionAtPeriodEnd(string customerEmail);
  }
}
