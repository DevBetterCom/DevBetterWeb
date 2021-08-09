using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities
{
  public class MemberSubscription : BaseEntity, IAggregateRoot
  {
    public DateTimeRange Dates { get; set; }
    public int MemberId { get; set; }
    public int MemberSubscriptionPlanId { get; set; }

    public MemberSubscription(int memberId, int memberSubscriptionPlanId, DateTimeRange dates)
    {
      MemberId = memberId;
      MemberSubscriptionPlanId = memberSubscriptionPlanId;
      Dates = dates;
    }
  }
}
