using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MembersNonSubscriptionSpec : Specification<Member>
{
  public MembersNonSubscriptionSpec()
  {
    Query
      .Where(member => member.MemberSubscriptions
	      .All(subscription => subscription.MemberId != member.Id));
  }
}

