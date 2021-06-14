using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;
using System;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public static class SubscriptionHelpers
  {
    public static MemberSubscription GetDefaultTestSubscription()
    {
      return new MemberSubscription
      {
        Dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(20))
      };
    }

    public static MemberSubscription GetSubscriptionWithPastEndDate()
    {
      return new MemberSubscription
      {
        Dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(-20))
      };
    }

    public static MemberSubscription GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(int daysToDate, int totalDays = 0)
    {
      int difference = totalDays - daysToDate;

      return new MemberSubscription
      {
        Dates = new DateTimeRange(DateTime.Today.AddDays(daysToDate * -1), DateTime.Today.AddDays(totalDays))
      };
    }
  }
}

