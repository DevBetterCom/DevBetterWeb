using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Pages.User;
using System.Linq;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class UserBooksUpdateModelHasReadBook
    {
        private Book Book { get; } = BookHelpers.GetDefaultTestBook();

        private UserBooksUpdateModel GetUserBooksUpdateModel()
        {
            return new UserBooksUpdateModel();
        }

        private UserBooksUpdateModel GetUserBooksUpdateModelWithBookInBooksRead()
        {
            UserBooksUpdateModel updateModel = new UserBooksUpdateModel();

            updateModel.BooksRead.Add(Book);

            return updateModel;
        }

        [Fact]
        public void ShouldReturnFalseIfNoBooksInBooksRead()
        {
            UserBooksUpdateModel updateModel = GetUserBooksUpdateModel();

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.False(hasReadBook);
        }

        [Fact]
        public void ShouldReturnTrueIfBooksReadContainsBook()
        {
            UserBooksUpdateModel updateModel = GetUserBooksUpdateModelWithBookInBooksRead();

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.True(hasReadBook);
        }

        [Fact]
        public void ShouldReturnTrueIfBooksReadContainsBookAndAnotherBook()
        {
            UserBooksUpdateModel updateModel = GetUserBooksUpdateModelWithBookInBooksRead();
            updateModel.BooksRead.Add(BookHelpers.GetDefaultTestBookWithSpecifiedTitle("C# for dummies"));

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.True(hasReadBook);
        }

        [Fact]
        public void ShouldReturnFalseIfBooksReadContainsOtherBookButNotBookPassed()
        {
            UserBooksUpdateModel updateModel = GetUserBooksUpdateModel();

            updateModel.BooksRead.Add(new DevBetterWeb.Core.Entities.Book { Author = "Steve Smith", Title = "C# stuff", Details = "A book about c#", PurchaseUrl = "https://buyabook.com" });

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.False(hasReadBook);
        }

    }
}
