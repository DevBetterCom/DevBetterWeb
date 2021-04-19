using System;

namespace DevBetterWeb.Core.Exceptions
{
  public class SubscriptionNotFoundException : Exception
  {
    public SubscriptionNotFoundException(int subscriptionId) : base($"No subscription found with id {subscriptionId}.")
    {
    }
  }
}
