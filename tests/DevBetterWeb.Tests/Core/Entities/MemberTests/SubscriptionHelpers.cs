using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;
using System;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public static class SubscriptionHelpers
  {
    public static Subscription GetDefaultTestSubscription()
    {
      return new Subscription
      {
        Dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(20))
      };
    }

    public static Subscription GetSubscriptionWithPastEndDate()
    {
      return new Subscription
      {
        Dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(-20))
      };
    }

    public static Subscription GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(int daysToDate, int totalDays = 0)
    {
      int difference = totalDays - daysToDate;

      return new Subscription
      {
        Dates = new DateTimeRange(DateTime.Today.AddDays(daysToDate * -1), DateTime.Today.AddDays(totalDays))
      };
    }
  }
}

