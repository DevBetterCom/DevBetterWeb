using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using System;
using System.Collections.Generic;

namespace DevBetterWeb.Web.Pages.User
{
  public class UserBillingViewModel
  {

    public List<BillingActivity> BillingActivities { get; private set; } = new List<BillingActivity>();
    public int TotalSubscribedDays { get; private set; }
    public string? SubscriptionPlanName { get; private set; }
    public BillingPeriod? BillingPeriod { get; private set; }
    public DateTime? CurrentSubscriptionEndDate { get; private set; }
    public DateTime? GraduationDate { get; private set; }
    public MemberSubscription? CurrentSubscription { get; private set; }

    public UserBillingViewModel(List<BillingActivity> billingActivities,
      int totalSubscribedDays,
      DateTime graduationDate)
    {
      BillingActivities = billingActivities;
      TotalSubscribedDays = totalSubscribedDays;
      GraduationDate = graduationDate;
    }


    public UserBillingViewModel(List<BillingActivity> billingActivities, 
      int totalSubscribedDays, 
      string subscriptionPlanName, 
      BillingPeriod billingPeriod,
      DateTime currentSubscriptionEndDate, 
      DateTime graduationDate,
      MemberSubscription currentSubscription)
    {
      BillingActivities = billingActivities;
      TotalSubscribedDays = totalSubscribedDays;
      SubscriptionPlanName = subscriptionPlanName;
      BillingPeriod = billingPeriod;
      CurrentSubscriptionEndDate = currentSubscriptionEndDate;
      GraduationDate = graduationDate;
      CurrentSubscription = currentSubscription;
    }
  }
}
