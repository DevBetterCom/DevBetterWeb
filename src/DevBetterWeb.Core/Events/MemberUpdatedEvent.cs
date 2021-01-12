using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
  public class MemberUpdatedEvent : BaseDomainEvent
  {
    public MemberUpdatedEvent(Member member, string updateDetails)
    {
      Member = member;
      UpdateDetails = updateDetails;
    }

    public Member Member { get; }
    public string UpdateDetails { get; internal set; }
  }


}
