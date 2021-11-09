using System;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using FluentAssertions;
using FluentAssertions.Execution;
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

    using (new AssertionScope())
    {
      billingActivity.Details.SubscriptionPlanName.Should().Be(subscriptionPlanName);
      billingActivity.Details.ActionVerbPastTense.Should().Be(actionVerb);
      billingActivity.Details.BillingPeriod.Should().Be(billingPeriod);
      billingActivity.Details.MemberName.Should().Be(member.UserFullName());
    }
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

    using (new AssertionScope())
    {
      billingActivity.Details.BillingPeriod.Should().Be(billingPeriod);
      billingActivity.Details.Amount.Should().Be(amount);
    }
  }
}
