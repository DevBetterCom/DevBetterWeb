using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class MemberSubscriptionsByMemberSpec : Specification<MemberSubscription>
    {
        public MemberSubscriptionsByMemberSpec(int memberId)
        {
            Query.Where(subscription => subscription.MemberId == memberId)
            .OrderBy(subscription => subscription.Dates.StartDate);
        }
    }
}

