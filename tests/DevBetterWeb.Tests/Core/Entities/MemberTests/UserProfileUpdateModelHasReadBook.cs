using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Pages.User;
using System.Linq;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class UserProfileUpdateModelHasReadBook
    {
        private Book Book { get; } = BookHelpers.GetDefaultBook();

        private UserProfileUpdateModel GetUserProfileUpdateModel()
        {
            return new UserProfileUpdateModel();
        }

        private UserProfileUpdateModel GetUserProfileUpdateModelWithBookInBooksRead()
        {
            UserProfileUpdateModel updateModel = new UserProfileUpdateModel();

            updateModel.BooksRead.Add(Book);

            return updateModel;
        }

        [Fact]
        public void ShouldReturnFalseIfNoBooksInBooksRead()
        {
            UserProfileUpdateModel updateModel = GetUserProfileUpdateModel();

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.False(hasReadBook);
        }

        [Fact]
        public void ShouldReturnTrueIfBooksReadContainsBook()
        {
            UserProfileUpdateModel updateModel = GetUserProfileUpdateModelWithBookInBooksRead();

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.True(hasReadBook);
        }

        [Fact]
        public void ShouldReturnTrueIfBooksReadContainsBookAndAnotherBook()
        {
            UserProfileUpdateModel updateModel = GetUserProfileUpdateModelWithBookInBooksRead();
            updateModel.BooksRead.Add(new DevBetterWeb.Core.Entities.Book { Author = "Steve Smith", Title = "C# stuff", Details = "A book about c#", PurchaseUrl = "https://buyabook.com" });

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.True(hasReadBook);
        }

        [Fact]
        public void ShouldReturnFalseIfBooksReadContainsOtherBookButNotBookPassed()
        {
            UserProfileUpdateModel updateModel = GetUserProfileUpdateModel();

            updateModel.BooksRead.Add(new DevBetterWeb.Core.Entities.Book { Author = "Steve Smith", Title = "C# stuff", Details = "A book about c#", PurchaseUrl = "https://buyabook.com" });

            bool hasReadBook = updateModel.HasReadBook(Book);

            Assert.False(hasReadBook);
        }

    }
}
