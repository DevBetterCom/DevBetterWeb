using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class BookCategoriesSpec : Specification<BookCategory>
{
  public BookCategoriesSpec()
  {
		Query
			.Include(bookCategory => bookCategory.Books!)
				.ThenInclude(book => book.MembersWhoHaveRead)
			.Include(bookCategory => bookCategory.Books!)
				.ThenInclude(book => book.MemberWhoUpload);
  }
}
