using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MemberByUserIdWithBillingActivitiesSpec : Specification<Member>, ISingleResultSpecification
{
	public MemberByUserIdWithBillingActivitiesSpec(string userId)
	{
		Query
			.AsNoTracking()
			.Where(member => member.UserId == userId)
			.Include(member => member.BillingActivities)
			.Include(member => member.MemberSubscriptions);

	}
}
