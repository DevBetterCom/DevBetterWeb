using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class MemberByUserIdSpec : BaseSpecification<Member>
    {
        public string UserId { get; }

        public MemberByUserIdSpec(string userId)
        {
            UserId = userId;

            AddCriteria(member => member.UserId == userId);
        }
    }
}
