using System;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberAddBillingActivity
{
  [Fact]
  public void AddsBillingActivityGivenRequiredParameters()
  {
    Member member = MemberHelpers.CreateWithDefaultConstructor();
    var subscriptionPlanName = Guid.NewGuid().ToString();
    var actionVerb = BillingActivityVerb.None;
    var billingPeriod = BillingPeriod.Month;

    member.AddBillingActivity(subscriptionPlanName, actionVerb, billingPeriod);

    var billingActivity = member.BillingActivities[0];

    billingActivity.Details.SubscriptionPlanName.ShouldBe(subscriptionPlanName);
    billingActivity.Details.ActionVerbPastTense.ShouldBe(actionVerb);
    billingActivity.Details.BillingPeriod.ShouldBe(billingPeriod);
    billingActivity.Details.MemberName.ShouldBe(member.UserFullName());
  }

  [Fact]
  public void AddsBillingActivityGivenMessageAndAmount()
  {
    Member member = MemberHelpers.CreateWithDefaultConstructor();
    var subscriptionPlanName = Guid.NewGuid().ToString();
    var actionVerb = BillingActivityVerb.None;
    var random = new Random();
    decimal amount = random.Next() / 100;
    var billingPeriod = BillingPeriod.Year;

    member.AddBillingActivity(subscriptionPlanName, actionVerb, billingPeriod, amount);

    var billingActivity = member.BillingActivities[0];

    billingActivity.Details.BillingPeriod.ShouldBe(billingPeriod);
    billingActivity.Details.Amount.ShouldBe(amount);
  }
}
