using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class SubscriptionsByMemberSpec : BaseSpecification<Subscription>
    {
        public SubscriptionsByMemberSpec(string userId)
        {
            AddCriteria(subscription => subscription.MemberId.ToString() == userId);
            ApplyOrderBy(subscription => subscription.StartDate);
        }
    }
}

