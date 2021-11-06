using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class MembersHavingUserIdsWithBooksSpec : Specification<Member>
{
  public MembersHavingUserIdsWithBooksSpec(IEnumerable<string> userIds)
  {
    Query.Where(member => userIds.Contains(member.UserId))
        .OrderByDescending(member => member.BooksRead!.Count)
        .ThenBy(member => member.LastName ?? "")
        .ThenBy(member => member.FirstName ?? "");

    Query.Include(member => member.BooksRead);
  }
}
