using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace DevBetterWeb.Web.Pages.User
{
  public class UserBillingViewModel
  {
    private const int DAYS_IN_YEAR = 365;
    private const int DAYS_SUBSCRIBED_TO_BECOME_ALUMNUS = 730;

    public List<BillingActivity> BillingActivities { get; private set; } = new List<BillingActivity>();
    public int TotalSubscribedDays { get; private set; }
    public SubscriptionPeriodEnum? SubscriptionPeriod { get; private set; }
    public DateTime? CurrentSubscriptionEndDate { get; private set; }
    public DateTime? GraduationDate { get; private set; }
    public Subscription? CurrentSubscription { get; private set; }

    public UserBillingViewModel(Member member)
    {
      BillingActivities = member.BillingActivities;
      TotalSubscribedDays = member.TotalSubscribedDays();

      foreach (var subscription in member.Subscriptions)
      {
        if (subscription.Dates.Contains(DateTime.Today))
        {
          CurrentSubscription = subscription;
        }
      }

      if(CurrentSubscription != null)
      {
        if(CurrentSubscription.Dates.ToDays(DateTime.Today) == DAYS_IN_YEAR)
        {
          SubscriptionPeriod = SubscriptionPeriodEnum.Yearly;
        }
        else 
        {
          SubscriptionPeriod = SubscriptionPeriodEnum.Monthly;
        }

        CurrentSubscriptionEndDate = CurrentSubscription.Dates.EndDate;

        var daysTillBecomingAlumnus = DAYS_SUBSCRIBED_TO_BECOME_ALUMNUS - TotalSubscribedDays;
        GraduationDate = DateTime.Today.AddDays(daysTillBecomingAlumnus);

      }
    }

    public enum SubscriptionPeriodEnum
    {
      Monthly,
      Yearly
    }
  }
}
