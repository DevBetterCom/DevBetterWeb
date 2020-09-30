using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class MemberAddBook
    {

        private Book GetBookToAdd()
        {
            return new Book
            {
                Title = "Book",
                Author = "Ilyana Smith",
                Details = "this is a book",
                PurchaseUrl = "https://buyabook.com"
            };
        }

        [Fact]
        public void ShouldAddBookGivenBook()
        {
            Member member = MemberHelpers.CreateWithDefaultConstructor();
            Book book = GetBookToAdd();

            member.AddBookRead(book);

            Assert.Contains(book, member.BooksRead);
        }

        [Fact]
        public void ShouldDoNothingGivenDuplicateBook()
        {
            Member member = MemberHelpers.CreateWithDefaultConstructor();
            Book book = GetBookToAdd();

            member.AddBookRead(book);
            member.AddBookRead(book);

            // if we get to this point no error was thrown.
            Assert.Contains(book, member.BooksRead);
            Assert.Single(member.BooksRead!);
        }

    }
}
