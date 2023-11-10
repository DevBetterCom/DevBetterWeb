using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MemberByUserIdWithBooksReadSpec : Specification<Member>, 
	ISingleResultSpecification<Member>
{
  public MemberByUserIdWithBooksReadSpec(string userId)
  {
	  Query
		  .Where(member => member.UserId == userId)
		  .Include(member => member.BooksRead)
				.ThenInclude(book => book.BookCategory);
  }
}
