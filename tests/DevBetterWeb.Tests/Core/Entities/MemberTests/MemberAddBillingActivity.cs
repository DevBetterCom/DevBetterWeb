using System;
using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public class MemberAddBillingActivity
  {
    [Fact]
    public void AddsBillingActivityGivenMessage()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      var message = Guid.NewGuid().ToString();

      member.AddBillingActivity(message);

      var billingActivity = member.BillingActivities[0];

      Assert.Equal(message, billingActivity.Details.Message);
    }

    [Fact]
    public void AddsBillingActivityGivenMessageAndAmount()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      var message = Guid.NewGuid().ToString();
      var random = new Random();
      decimal amount = random.Next() / 100;

      member.AddBillingActivity(message, amount);

      var billingActivity = member.BillingActivities[0];

      Assert.Equal(message, billingActivity.Details.Message);
      Assert.Equal(amount, billingActivity.Details.Amount);
    }
  }
}
