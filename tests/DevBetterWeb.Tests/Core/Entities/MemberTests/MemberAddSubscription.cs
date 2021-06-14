using DevBetterWeb.Core.Entities;
using Xunit;
using System.Linq;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public class MemberAddSubscription
  {
    [Fact]
    public void AddsSubscriptionWithGivenDateTimeRange()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      MemberSubscription subscription = SubscriptionHelpers.GetDefaultTestSubscription();

      member.AddSubscription(subscription.Dates);

      var subscriptionAdded = member.Subscriptions.Any(s => s.Dates.Equals(subscription.Dates));

      Assert.True(subscriptionAdded);
    }
  }
}
