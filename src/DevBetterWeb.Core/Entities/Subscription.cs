using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities
{
  public class Subscription : BaseEntity, IAggregateRoot
  {
    //public DateTime StartDate { get; set; }
    //public DateTime? EndDate { get; set; }
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public DateTimeRange Dates { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public int MemberId { get; set; }
    public int SubscriptionPlanId { get; set; }
  }
}
