using System;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.ValueObjects;
using DevBetterWeb.UnitTests.Core.Entities;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Events;

public class BillingActivityCreatedEventConstructor
{
  private readonly Member _member = MemberHelpers.CreateWithDefaultConstructor();
  private readonly BillingActivity _billingActivity;

  public BillingActivityCreatedEventConstructor()
  {
    var subscriptionPlanName = Guid.NewGuid().ToString();
    var actionVerb = BillingActivityVerb.None;
    var billingPeriod = BillingPeriod.Month;
  
    _billingActivity = new BillingActivity(_member.Id, new BillingDetails(_member.UserFullName(), subscriptionPlanName, actionVerb, billingPeriod, DateTime.UtcNow));
  }

  [Fact]
  public void ShouldSetValues()
  {
    var sut = new BillingActivityCreatedEvent(_billingActivity, _member);

    sut.DateOccurred.ShouldNotBe(default);
    sut.Member.ShouldBe(_member);
    sut.BillingActivity.ShouldBe(_billingActivity);
  }
    
  [Fact]
  public void ShouldThrowExceptionWhenBillingActivityIsNull()
  {
    var action = () => new BillingActivityCreatedEvent(null!, _member);

    action.ShouldThrow<ArgumentNullException>();
  }

  [Fact]
  public void ShouldThrowExceptionWhenMemberIsNull()
  {
    var action = () => new BillingActivityCreatedEvent(_billingActivity, null!);

    action.ShouldThrow<ArgumentNullException>();
  }
}
