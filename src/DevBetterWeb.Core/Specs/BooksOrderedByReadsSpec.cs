using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class BooksOrderedByReadsSpec : Specification<Book>
{
  public BooksOrderedByReadsSpec()
  {
		Query
			.Include(book => book.MemberWhoUpload)
			.Include(book => book.MembersWhoHaveRead)
			.Include(book => book.BookCategory);
  }
}
