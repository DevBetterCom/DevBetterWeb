using DevBetterWeb.Core.ValueObjects;
using Xunit;
using DevBetterWeb.Core.Enums;

namespace DevBetterWeb.Tests.Core.ValueObjects
{
  public class BillingDetailsGetMessageForMemberView : BillingDetailsGetMessageTest
  {
    [Fact]
    public void ReturnsCorrectMessageGivenSubscribedAction()
    {
      BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, "Subscribed", BillingPeriod.Month, _testDate, _amount);

      string expectedMessage = $"You Subscribed to {_subscriptionPlanName} for ${_amount} on {_testDate.ToLongDateString()}.";
      var message = details.GetMessageForMemberView();

      Assert.Equal(expectedMessage, message);
    }

    [Fact]
    public void ReturnsCorrectMessageGivenRenewedAction()
    {
      BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, "Renewed", BillingPeriod.Month, _testDate, _amount);

      string expectedMessage = $"You Renewed {_subscriptionPlanName} for ${_amount} on {_testDate.ToLongDateString()}.";
      var message = details.GetMessageForMemberView();

      Assert.Equal(expectedMessage, message);
    }

    [Fact]
    public void ReturnsCorrectMessageGivenCancelledAction()
    {
      BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, "Cancelled", BillingPeriod.Month, _testDate);

      string expectedMessage = $"You Cancelled {_subscriptionPlanName} on {_testDate.ToLongDateString()}.";
      var message = details.GetMessageForMemberView();

      Assert.Equal(expectedMessage, message);
    }

    [Fact]
    public void ReturnsCorrectMessageGivenEndedAction()
    {
      BillingDetails details = new BillingDetails(_memberName, _subscriptionPlanName, "Ended", BillingPeriod.Month, _testDate);

      string expectedMessage = $"Your {_subscriptionPlanName} Ended on {_testDate.ToLongDateString()}.";
      var message = details.GetMessageForMemberView();

      Assert.Equal(expectedMessage, message);
    }
  }
}
