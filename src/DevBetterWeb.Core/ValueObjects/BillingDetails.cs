using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DevBetterWeb.Core.Enums;
using System;

namespace DevBetterWeb.Core.ValueObjects
{
  public class BillingDetails : ValueObject
  {
    public decimal Amount { get; }
    public string MemberName { get; }
    public string SubscriptionPlanName { get; }
    public BillingActivityVerb ActionVerbPastTense { get; }
    public BillingPeriod BillingPeriod { get; }
    public DateTime Date { get; }

    public BillingDetails(string memberName, string subscriptionPlanName, BillingActivityVerb actionVerbPastTense, BillingPeriod billingPeriod, DateTime date, decimal amount = 0)
    {
      Amount = amount;
      MemberName = memberName;
      SubscriptionPlanName = subscriptionPlanName;
      ActionVerbPastTense = actionVerbPastTense;
      BillingPeriod = billingPeriod;
      Date = date;
    }

    public string GetMessageForMemberView()
    {
      string message = GetSentenceWithProperSyntax(Viewer.Member);

      return message;
    }

    public string GetMessageForAdminView(string memberId)
    {
      string message = GetSentenceWithProperSyntax(Viewer.Admin, memberId);
      return message;
    }

    private string GetSentenceWithProperSyntax(Viewer viewer, string memberId = "")
    {
      string message = "";

      switch(viewer)
      {
        case Viewer.Admin:
          message += $"Member ID: {memberId}. {MemberName}";
          break;
        case Viewer.Member:
          message += $"You";
          break;
        default:
          break;
      }

      switch (ActionVerbPastTense)
      {
        case BillingActivityVerb.Subscribed:
          message += $" {ActionVerbPastTense.ToString()} to {SubscriptionPlanName} for ${Amount}";
          break;
        case BillingActivityVerb.Ended:
          message += $"{GetPossessiveEnding(viewer)} {SubscriptionPlanName} {ActionVerbPastTense.ToString()}";
          break;
        case BillingActivityVerb.Renewed:
          message += $" {ActionVerbPastTense.ToString()} {SubscriptionPlanName} for ${Amount}";
          break;
        case BillingActivityVerb.Cancelled:
        default:
          message += $" {ActionVerbPastTense.ToString()} {SubscriptionPlanName}";
          break;
      }

      message += $" on {Date.ToLongDateString()}.";

      return message;
    }

    private string GetPossessiveEnding(Viewer viewer)
    {
      switch(viewer)
      {
        case Viewer.Admin:
          return "'s";
        case Viewer.Member:
          return "r";
        default:
          return "";
      }
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
      ActionVerbPastTense = BillingActivityVerb.None;
      BillingPeriod = BillingPeriod.None;
    }

    private enum Viewer
    {
      Member,
      Admin
    }
  }
}
