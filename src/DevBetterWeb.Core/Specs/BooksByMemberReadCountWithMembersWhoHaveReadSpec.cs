using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class BooksByMemberReadCountWithMembersWhoHaveReadSpec : Specification<Book>
{
  public BooksByMemberReadCountWithMembersWhoHaveReadSpec()
  {
    Query
	    .OrderByDescending(book => book.MembersWhoHaveRead!.Count)
      .ThenBy(book => book.Title ?? "");
    Query
	    .Include(book => book.MembersWhoHaveRead)
	    .Include(book => book.BookCategory);
	}
}
