using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class MemberByUserIdWithBillingActivitiesSpec : Specification<Member>, ISingleResultSpecification
{
  public MemberByUserIdWithBillingActivitiesSpec(string userId)
  {
    Query.Where(member => member.UserId == userId);
    Query.Include(member => member.BillingActivities);
  }
}
