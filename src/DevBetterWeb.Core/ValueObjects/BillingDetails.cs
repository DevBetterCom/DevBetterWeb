using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DevBetterWeb.Core.Enums;

namespace DevBetterWeb.Core.ValueObjects
{
  public class BillingDetails : ValueObject
  {
    public decimal Amount { get; }
    public string MemberName { get; }
    public string SubscriptionPlanName { get; }
    public string ActionVerbPastTense { get; }
    public BillingPeriod BillingPeriod { get; }

    public BillingDetails(string memberName, string subscriptionPlanName, string actionVerbPastTense, BillingPeriod billingPeriod, decimal amount = 0)
    {
      Amount = amount;
      MemberName = memberName;
      SubscriptionPlanName = subscriptionPlanName;
      ActionVerbPastTense = actionVerbPastTense;
      BillingPeriod = billingPeriod;
    }

    public string GetMessageForMemberView()
    {
      string message = $"You {ActionVerbPastTense} ";

      return message;
    }

    public string GetMessageForAdminView(string memberId)
    {
      string message = $"Member {MemberName} with id {memberId}";
      return message;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Amount;
      yield return SubscriptionPlanName;
      yield return MemberName;
      yield return ActionVerbPastTense;
      yield return BillingPeriod;
    }

    public BillingDetails()
    {
      Amount = 0;
      MemberName = "";
      SubscriptionPlanName = "";
      ActionVerbPastTense = "";
      BillingPeriod = BillingPeriod.None;
    }
  }
}
