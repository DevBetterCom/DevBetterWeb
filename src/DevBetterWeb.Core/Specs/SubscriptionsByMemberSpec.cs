using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class SubscriptionsByMemberSpec : BaseSpecification<Subscription>
    {
        public SubscriptionsByMemberSpec(int memberId)
        {
            AddCriteria(subscription => subscription.MemberId == memberId);
            ApplyOrderBy(subscription => subscription.StartDate);
        }
    }
}

