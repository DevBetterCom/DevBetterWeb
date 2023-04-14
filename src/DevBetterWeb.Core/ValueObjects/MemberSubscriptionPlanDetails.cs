using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DevBetterWeb.Core.Enums;

namespace DevBetterWeb.Core.ValueObjects;

public class MemberSubscriptionPlanDetails : ValueObject
{
  public string Name { get; set; }
  public decimal PricePerBillingPeriod { get; set; }
  public BillingPeriod BillingPeriod { get; set; }

  public MemberSubscriptionPlanDetails(string name, decimal pricePerBillingPeriod, BillingPeriod billingPeriod)
  {
    Name = name;
    PricePerBillingPeriod = pricePerBillingPeriod;
    BillingPeriod = billingPeriod;
  }

  public MemberSubscriptionPlanDetails()
  {
    Name = "";
    PricePerBillingPeriod = 0;
    BillingPeriod = BillingPeriod.None;
  }

  protected override IEnumerable<IComparable> GetEqualityComponents()
  {
    yield return PricePerBillingPeriod;
    yield return BillingPeriod;
    yield return Name;
  }
}
