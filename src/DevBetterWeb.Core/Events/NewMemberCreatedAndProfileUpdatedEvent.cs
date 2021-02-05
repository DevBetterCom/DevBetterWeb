using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
  public class NewMemberCreatedAndProfileUpdatedEvent : BaseDomainEvent
  {
    public NewMemberCreatedAndProfileUpdatedEvent(Member member)
    {
      Member = member;
    }

    public Member Member { get; }
  }
}
