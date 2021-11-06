using System;

namespace DevBetterWeb.Core.Exceptions;

public class MemberSubscriptionNotFoundException : Exception
{
  public MemberSubscriptionNotFoundException(int subscriptionId) : base($"No subscription found with id {subscriptionId}.")
  {
  }
}
