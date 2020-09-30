using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class MemberAddBook
    {

        [Fact]
        public void ShouldAddBookGivenBook()
        {
            Member member = MemberHelpers.CreateWithDefaultConstructor();
            Book book = BookHelpers.GetDefaultBook();

            member.AddBookRead(book);

            Assert.Contains(book, member.BooksRead);
        }

        [Fact]
        public void ShouldDoNothingGivenDuplicateBook()
        {
            Member member = MemberHelpers.CreateWithDefaultConstructor();
            Book book = BookHelpers.GetDefaultBook();

            member.AddBookRead(book);
            member.AddBookRead(book);

            // if we get to this point no error was thrown.
            Assert.Contains(book, member.BooksRead);
            Assert.Single(member.BooksRead!);
        }

    }
}
