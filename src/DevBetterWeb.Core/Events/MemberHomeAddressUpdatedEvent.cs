using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class MemberHomeAddressUpdatedEvent : BaseDomainEvent
{
  public MemberHomeAddressUpdatedEvent(Member member, string updateDetails)
  {
    Member = member;
    UpdateDetails = updateDetails;
  }

  public Member Member { get; }
  public string UpdateDetails { get; internal set; }
}
