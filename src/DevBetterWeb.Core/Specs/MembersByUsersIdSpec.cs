using System.Collections.Generic;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MembersByUsersIdSpec : Specification<Member>
{
  public MembersByUsersIdSpec(List<string> userIds)
  {
	  Query
		.AsNoTracking()
		.Where(member => userIds.Contains(member.UserId))
		.Include(m => m.MemberSubscriptions)
		.OrderBy(member => member.LastName);
  }
}
