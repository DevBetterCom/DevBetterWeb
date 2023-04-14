using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DevBetterWeb.Core.Enums;

namespace DevBetterWeb.Core.ValueObjects;

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

    switch (viewer)
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

		message += ActionVerbPastTense switch
		{
			BillingActivityVerb.Subscribed => $" {ActionVerbPastTense} to {SubscriptionPlanName} for ${Amount}",
			BillingActivityVerb.Ended => $"{GetPossessiveEnding(viewer)} {SubscriptionPlanName} {ActionVerbPastTense}",
			BillingActivityVerb.Renewed => $" {ActionVerbPastTense} {SubscriptionPlanName} for ${Amount}",
			_ => $" {ActionVerbPastTense} {SubscriptionPlanName}",
		};
		message += $" on {Date.ToLongDateString()}.";

    return message;
  }

  private static string GetPossessiveEnding(Viewer viewer)
  {
		return viewer switch
		{
			Viewer.Admin => "'s",
			Viewer.Member => "r",
			_ => "",
		};
	}

  protected override IEnumerable<IComparable> GetEqualityComponents()
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
