using System;
using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests
{
  public class MemberExtendCurrentSubscription
  {
    [Fact]
    public void ExtendsSubscription()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      MemberSubscription starterSubscription = SubscriptionHelpers.GetDefaultTestSubscription();
      DateTime newEndDate = DateTime.Now.AddDays(30);
      DateTime originalStartDate = starterSubscription.Dates.StartDate;

      member.AddSubscription(starterSubscription.Dates, 1);

      member.ExtendCurrentSubscription(newEndDate);

      Assert.Equal(newEndDate, member.MemberSubscriptions[0].Dates.EndDate);
      Assert.Equal(originalStartDate, member.MemberSubscriptions[0].Dates.StartDate);

      Assert.Single(member.MemberSubscriptions);
    }

    [Fact]
    public void DoesNothingGivenNoCurrentSubscription()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      MemberSubscription starterSubscription = SubscriptionHelpers.GetSubscriptionWithPastEndDate();
      DateTime newEndDate = DateTime.Now.AddDays(30);
      var originalEndDate = starterSubscription.Dates.EndDate;

      member.AddSubscription(starterSubscription.Dates, 1);

      member.ExtendCurrentSubscription(newEndDate);

      Assert.Equal(originalEndDate, member.MemberSubscriptions[0].Dates.EndDate);

      Assert.Single(member.MemberSubscriptions);
    }
  }
}
