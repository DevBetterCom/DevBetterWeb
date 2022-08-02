using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class BookByIdWithMembersSpec : Specification<Book>, ISingleResultSpecification
{
  public BookByIdWithMembersSpec(int bookId)
  {
	  Query
		  .Where(book => book.Id == bookId)
		  .Include(book => book.MembersWhoHaveRead)
		  .Include(book => book.BookCategory);
  }
}
