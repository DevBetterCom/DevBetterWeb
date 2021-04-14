using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
  public class MemberByUserIdWithBooksReadSpec : Specification<Member>
  {
    public MemberByUserIdWithBooksReadSpec(string userId)
    {
      Query.Where(member => member.UserId == userId)
        .Include(member => member.BooksRead);
    }
  }
}
