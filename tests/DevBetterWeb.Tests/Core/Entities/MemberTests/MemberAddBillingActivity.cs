using System;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public class MemberAddBillingActivity
  {
    [Fact]
    public void AddsBillingActivityGivenRequiredParameters()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      var subscriptionPlanName = Guid.NewGuid().ToString();
      var actionVerb = Guid.NewGuid().ToString();
      var billingPeriod = BillingPeriod.Month;

      member.AddBillingActivity(subscriptionPlanName, actionVerb, billingPeriod);

      var billingActivity = member.BillingActivities[0];

      Assert.Equal(subscriptionPlanName, billingActivity.Details.SubscriptionPlanName);
      Assert.Equal(actionVerb, billingActivity.Details.ActionVerbPastTense);
      Assert.Equal(billingPeriod, billingActivity.Details.BillingPeriod);
      Assert.Equal(member.UserFullName(), billingActivity.Details.MemberName);
    }

    [Fact]
    public void AddsBillingActivityGivenMessageAndAmount()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      var subscriptionPlanName = Guid.NewGuid().ToString();
      var actionVerb = Guid.NewGuid().ToString();
      var random = new Random();
      decimal amount = random.Next() / 100;
      var billingPeriod = BillingPeriod.Year;

      member.AddBillingActivity(subscriptionPlanName, actionVerb, billingPeriod, amount);

      var billingActivity = member.BillingActivities[0];

      Assert.Equal(billingPeriod, billingActivity.Details.BillingPeriod);
      Assert.Equal(amount, billingActivity.Details.Amount);
    }
  }
}
