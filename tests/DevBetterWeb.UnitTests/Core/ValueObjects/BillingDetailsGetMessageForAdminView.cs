using System;
using DevBetterWeb.Core.Enums;
using DevBetterWeb.Core.ValueObjects;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.ValueObjects;

public class BillingDetailsGetMessageForAdminView : BillingDetailsGetMessageTest
{
  private string _memberId = Guid.NewGuid().ToString();

  [Fact]
  public void ReturnsCorrectMessageGivenSubscribedAction()
  {
    BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, BillingActivityVerb.Subscribed, BillingPeriod.Month, _testDate, _amount);

    string expectedMessage = $"Member ID: {_memberId}. {_memberName} Subscribed to {_subscriptionPlanName} for ${_amount} on {_testDate.ToLongDateString()}.";
    var message = details.GetMessageForAdminView(_memberId);

    Assert.Equal(expectedMessage, message);
  }

  [Fact]
  public void ReturnsCorrectMessageGivenRenewedAction()
  {
    BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, BillingActivityVerb.Renewed, BillingPeriod.Month, _testDate, _amount);

    string expectedMessage = $"Member ID: {_memberId}. {_memberName} Renewed {_subscriptionPlanName} for ${_amount} on {_testDate.ToLongDateString()}.";
    var message = details.GetMessageForAdminView(_memberId);

    Assert.Equal(expectedMessage, message);
  }

  [Fact]
  public void ReturnsCorrectMessageGivenCancelledAction()
  {
    BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, BillingActivityVerb.Cancelled, BillingPeriod.Month, _testDate);

    string expectedMessage = $"Member ID: {_memberId}. {_memberName} Cancelled {_subscriptionPlanName} on {_testDate.ToLongDateString()}.";
    var message = details.GetMessageForAdminView(_memberId);

    Assert.Equal(expectedMessage, message);
  }

  [Fact]
  public void ReturnsCorrectMessageGivenEndedAction()
  {
    BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, BillingActivityVerb.Ended, BillingPeriod.Month, _testDate);

    string expectedMessage = $"Member ID: {_memberId}. {_memberName}'s {_subscriptionPlanName} Ended on {_testDate.ToLongDateString()}.";
    var message = details.GetMessageForAdminView(_memberId);

    Assert.Equal(expectedMessage, message);
  }
}
