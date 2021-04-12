using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities
{
  public class SubscriptionPlan : BaseEntity
  {
    public SubscriptionPlanDetails? Details { get; set; }

  }
}
