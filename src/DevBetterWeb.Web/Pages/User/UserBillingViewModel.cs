using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace DevBetterWeb.Web.Pages.User
{
  public class UserBillingViewModel
  {
    private const int DAYS_IN_YEAR = 365;

    private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public UserBillingViewModel(IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService)
    {
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
    }

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

      CurrentSubscription = _memberSubscriptionPeriodCalculationsService!.GetCurrentSubscription(member);

      if (CurrentSubscription.Dates.ToDays(DateTime.Today) == DAYS_IN_YEAR)
      {
        SubscriptionPeriod = SubscriptionPeriodEnum.Yearly;
      }
      else
      {
        SubscriptionPeriod = SubscriptionPeriodEnum.Monthly;
      }

      CurrentSubscriptionEndDate = _memberSubscriptionPeriodCalculationsService.GetCurrentSubscriptionEndDate(member);

      GraduationDate = _memberSubscriptionPeriodCalculationsService.GetGraduationDate(member);
    }

    public enum SubscriptionPeriodEnum
    {
      Monthly,
      Yearly
    }
  }
}
