using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class SubscriptionsByMemberSpec : Specification<MemberSubscription>
    {
        public SubscriptionsByMemberSpec(int memberId)
        {
            Query.Where(subscription => subscription.MemberId == memberId)
            .OrderBy(subscription => subscription.Dates.StartDate);
        }
    }
}

