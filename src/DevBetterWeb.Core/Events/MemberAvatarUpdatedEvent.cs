using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
  public class MemberAvatarUpdatedEvent : BaseDomainEvent
  {
    public MemberAvatarUpdatedEvent(Member member)
    {
      Member = member;
    }

    public Member Member { get; }
  }


}
