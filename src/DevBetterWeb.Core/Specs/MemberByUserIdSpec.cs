using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class MemberByUserIdSpec : Specification<Member>
    {
        public string UserId { get; }

        public MemberByUserIdSpec(string userId)
        {
            UserId = userId;

            Query.Where(member => member.UserId == userId);
        }
    }
}
