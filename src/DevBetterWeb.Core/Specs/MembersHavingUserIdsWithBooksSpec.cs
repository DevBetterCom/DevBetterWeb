using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MembersHavingUserIdsWithBooksSpec : Specification<Member>
{
	public MembersHavingUserIdsWithBooksSpec(IEnumerable<string> userIds)
	{
		Query
			.Where(member => userIds.Contains(member.UserId))
			.Where(m => m.BooksRead.Count > 0)
			.OrderByDescending(member => member.BooksRead!.Count)
			.ThenBy(member => member.LastName ?? "")
			.ThenBy(member => member.FirstName ?? "");

		Query
			.Include(member => member.BooksRead);
	}

	public MembersHavingUserIdsWithBooksSpec(IEnumerable<string> userIds, List<int> excludedAlumniMembersIds) : this(userIds)
	{
		Query
			.Where(m => excludedAlumniMembersIds.All(alumni => alumni != m.Id));

	}
}
