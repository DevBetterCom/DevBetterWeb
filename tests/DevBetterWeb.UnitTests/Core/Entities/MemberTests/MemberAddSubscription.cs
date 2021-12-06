using System.Linq;
using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberAddSubscription
{
  [Fact]
  public void AddsSubscriptionWithGivenDateTimeRange()
  {
    Member member = MemberHelpers.CreateWithDefaultConstructor();
    MemberSubscription subscription = SubscriptionHelpers.GetDefaultTestSubscription();

    member.AddSubscription(subscription.Dates, 1);

    var subscriptionAdded = member.MemberSubscriptions.Any(s => s.Dates.Equals(subscription.Dates));

    Assert.True(subscriptionAdded);
  }
}
