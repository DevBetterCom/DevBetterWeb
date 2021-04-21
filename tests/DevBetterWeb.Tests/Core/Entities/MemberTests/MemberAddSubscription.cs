using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public class MemberAddSubscription
  {
    [Fact]
    public void AddsSubscription()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      MemberSubscription subscription = SubscriptionHelpers.GetDefaultTestSubscription();

      member.AddSubscription(subscription);

      Assert.Contains(subscription, member.Subscriptions);
    }

    [Fact]
    public void DoesNothingGivenSubscriptionAlreadyInList()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      MemberSubscription subscription = SubscriptionHelpers.GetDefaultTestSubscription();

      member.AddSubscription(subscription);

      Assert.Contains(subscription, member.Subscriptions);

      member.AddSubscription(subscription);

      Assert.Single(member.Subscriptions);
    }
  }
}
