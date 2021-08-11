using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities
{
  // mainly here for UI in Billing view for User Profile, needs to correspond to things in Stripe
  public class MemberSubscriptionPlan : BaseEntity, IAggregateRoot
  {
    public MemberSubscriptionPlanDetails Details { get; set; }

    public MemberSubscriptionPlan(MemberSubscriptionPlanDetails details)
    {
      Details = details;
    }

    public MemberSubscriptionPlan()
    {
      Details = new MemberSubscriptionPlanDetails();
    }
  }
}
