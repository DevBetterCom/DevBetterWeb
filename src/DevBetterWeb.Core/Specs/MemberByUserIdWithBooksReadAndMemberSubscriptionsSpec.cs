using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
  public class MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec : Specification<Member>, ISingleResultSpecification
  {
    public MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec(string userId)
    {
      Query.Where(member => member.UserId == userId);
      Query.Include(member => member.BooksRead);
      Query.Include(member => member.MemberSubscriptions);
    }
  }
}
