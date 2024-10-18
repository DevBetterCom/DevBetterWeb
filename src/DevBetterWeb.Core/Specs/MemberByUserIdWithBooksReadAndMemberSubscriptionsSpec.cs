using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec : Specification<Member>, 
	ISingleResultSpecification<Member>
{
  public MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec(string userId)
  {
    Query
	    .Where(member => member.UserId == userId)
      .Include(member => member.BooksRead)
				.ThenInclude(book => book.BookCategory);

    Query
	    .Include(member => member.MemberSubscriptions);

		Query
			.Include(member => member.UploadedBooks)
			.AsSplitQuery()
			.AsNoTracking();
	}
}
