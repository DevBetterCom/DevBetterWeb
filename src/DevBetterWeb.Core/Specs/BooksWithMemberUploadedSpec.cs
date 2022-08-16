using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class BooksWithMemberUploadedSpec : Specification<Book>
{
  public BooksWithMemberUploadedSpec()
  {
		Query
			.OrderBy(book => book.Title ?? string.Empty)
			.Include(book => book.BookCategory)
			.Include(book => book.MemberWhoUpload);
	}
}
