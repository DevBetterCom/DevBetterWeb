using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DevBetterWeb.Core.Enums;

namespace DevBetterWeb.Core.ValueObjects
{
  public class SubscriptionPlanDetails : ValueObject
  {
    public string Name { get; set; }
    public decimal PricePerBillingPeriod { get; set; }
    public BillingPeriod BillingPeriod { get; set; }

    public SubscriptionPlanDetails(string name, decimal pricePerBillingPeriod, BillingPeriod billingPeriod)
    {
      Name = name;
      PricePerBillingPeriod = pricePerBillingPeriod;
      BillingPeriod = billingPeriod;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return PricePerBillingPeriod;
      yield return BillingPeriod;
      yield return Name;
    }
  }
}
