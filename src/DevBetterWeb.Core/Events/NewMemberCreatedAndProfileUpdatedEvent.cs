using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

// TODO: Shady, this appears to never be used. Should it be removed?
public class NewMemberCreatedAndProfileUpdatedEvent : BaseDomainEvent
{
  public NewMemberCreatedAndProfileUpdatedEvent(Member member)
  {
    Member = member;
  }

  public Member Member { get; }
}
