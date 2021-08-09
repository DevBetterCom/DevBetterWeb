using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;
using System;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public static class SubscriptionHelpers
  {
    public const int TEST_MEMBER_ID = 1;
    public const int TEST_MEMBER_PLAN_ID = 2;
    public static MemberSubscription GetDefaultTestSubscription()
    {
      var dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(20));
      return new MemberSubscription(TEST_MEMBER_ID, TEST_MEMBER_PLAN_ID, dates);
    }

    public static MemberSubscription GetSubscriptionWithPastEndDate()
    {
      var dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(-20));
      return new MemberSubscription(TEST_MEMBER_ID, TEST_MEMBER_PLAN_ID, dates);
    }

    public static MemberSubscription GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(int daysToDate, int totalDays = 0)
    {
      int difference = totalDays - daysToDate;
      var dates = new DateTimeRange(DateTime.Today.AddDays(daysToDate * -1), DateTime.Today.AddDays(totalDays));
      return new MemberSubscription(TEST_MEMBER_ID, TEST_MEMBER_PLAN_ID, dates);
    }
  }
}

