using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class MemberByUserIdWithBooksReadSpec : Specification<Member>
    {
        public string UserId { get; }

        public MemberByUserIdWithBooksReadSpec(string userId)
        {
            UserId = userId;

            Query.Where(member => member.UserId == userId);
            Query.Include("BooksRead");
                //.ThenInclude(booksread => booksread.Book);
        }
    }
}
