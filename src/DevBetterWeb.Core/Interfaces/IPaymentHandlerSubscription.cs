using System;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerSubscription
  {
    DateTimeRange GetDateTimeRange(string subscriptionId);
    string GetStatus(string subscriptionId);
    string GetCustomerId(string subscriptionId);
  }
}
