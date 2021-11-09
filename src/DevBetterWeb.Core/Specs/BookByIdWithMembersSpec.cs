using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class BookByIdWithMembersSpec : Specification<Book>, ISingleResultSpecification
{
  public BookByIdWithMembersSpec(int bookId)
  {
    Query.Where(book => book.Id == bookId);
    Query.Include(book => book.MembersWhoHaveRead);
  }
}
