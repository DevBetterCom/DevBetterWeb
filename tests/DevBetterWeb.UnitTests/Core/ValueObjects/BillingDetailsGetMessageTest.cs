using System;

namespace DevBetterWeb.UnitTests.Core.ValueObjects
{
  public class BillingDetailsGetMessageTest
  {
    protected string _memberName = "MemberNameTest";
    protected string _subscriptionPlanName = "SubscriptionPlanNameTest";
    protected DateTime _testDate = DateTime.Now.AddDays(-30);
    protected decimal _amount = 200.00M;
  }
}
