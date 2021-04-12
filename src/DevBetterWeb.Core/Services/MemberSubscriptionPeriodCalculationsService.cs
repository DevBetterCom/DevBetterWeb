﻿using System;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Core.Services
{
  public class MemberSubscriptionPeriodCalculationsService : IMemberSubscriptionPeriodCalculationsService
  {
    private const int DAYS_SUBSCRIBED_TO_BECOME_ALUMNUS = 730;

    public bool GetHasCurrentSubscription(Member member)
    {
      bool hasCurrentSubscription = false;

      foreach (var subscription in member.Subscriptions)
      {
        if (subscription.Dates.Contains(DateTime.Today))
        {
          hasCurrentSubscription = true;
        }
      }
      return hasCurrentSubscription;
    }

    // none of these methods should ever be called if member does not have current subscription
    public MemberSubscription GetCurrentSubscription(Member member)
    {

      foreach (var subscription in member.Subscriptions)
      {
        if (subscription.Dates.Contains(DateTime.Today))
        {
          return subscription;
        }
      }

      throw new Exceptions.NoCurrentSubscriptionFoundException();
    }

    public DateTime GetCurrentSubscriptionEndDate(Member member)
    {
      var currentSubscription = GetCurrentSubscription(member);

      var endDate = currentSubscription.Dates.EndDate ?? DateTime.MinValue;

      return endDate;
    }

    public DateTime GetGraduationDate(Member member)
    {
      var totalSubscribedDays = member.TotalSubscribedDays();

      var daysTillBecomingAlumnus = DAYS_SUBSCRIBED_TO_BECOME_ALUMNUS - totalSubscribedDays;
      var graduationDate = DateTime.Today.AddDays(daysTillBecomingAlumnus);

      return graduationDate;
    }
  }
}
