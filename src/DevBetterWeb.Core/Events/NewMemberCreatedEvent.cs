using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class NewMemberCreatedEvent : BaseDomainEvent
    {
        public NewMemberCreatedEvent(Member member)
        {
            Member = member;
        }

        public Member Member { get; }
    }
}
