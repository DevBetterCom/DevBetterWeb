using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Events;

public class MemberAddressUpdatedEvent : BaseDomainEvent
{
  public MemberAddressUpdatedEvent(Member member, Address? oldAddress)
  {
    Member = member;
    OldAddress = oldAddress;
  }

  public Member Member { get; }
  public Address? OldAddress { get; }
}
