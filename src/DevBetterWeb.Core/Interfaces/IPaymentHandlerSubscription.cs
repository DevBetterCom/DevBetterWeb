using System;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerSubscription
  {
    DateTime GetStartDate(string subscriptionId);
    DateTime GetEndDate(string subscriptionId);
  }
}
