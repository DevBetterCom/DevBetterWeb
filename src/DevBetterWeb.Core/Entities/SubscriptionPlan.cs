using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities
{
  public class SubscriptionPlan : BaseEntity, IAggregateRoot
  {
    public SubscriptionPlanDetails Details { get; set; }

    public SubscriptionPlan(SubscriptionPlanDetails details)
    {
      Details = details;
    }

    public SubscriptionPlan()
    {
      Details = new SubscriptionPlanDetails();
    }
  }
}
