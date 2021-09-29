using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests
{
  public class MemberTotalSubscribedDays
  {
    [Fact]
    public void ReturnsDaysSubscribedToDate()
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      MemberSubscription subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(50);

      member.AddSubscription(subscription.Dates, 1);
      int days = member.TotalSubscribedDays();

      Assert.Equal(50, days);
    }

    [Fact]
    public void ReturnsDaysSubscribedToDateWithoutDaysAfterToday()
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      MemberSubscription subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(58, 12);

      member.AddSubscription(subscription.Dates, 1);
      int days = member.TotalSubscribedDays();

      Assert.Equal(58, days);
    }
  }
}
