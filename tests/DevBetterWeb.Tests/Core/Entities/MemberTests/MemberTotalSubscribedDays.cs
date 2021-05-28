using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public class MemberTotalSubscribedDays
  {
    [Fact]
    public void ReturnsDaysSubscribedToDate()
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      Subscription subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(50);

      member.AddSubscription(subscription.Dates);
      int days = member.TotalSubscribedDays();

      Assert.Equal(50, days);
    }

    [Fact]
    public void ReturnsDaysSubscribedToDateWithoutDaysAfterToday()
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      Subscription subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(58, 12);

      member.AddSubscription(subscription.Dates);
      int days = member.TotalSubscribedDays();

      Assert.Equal(58, days);
    }
  }
}
