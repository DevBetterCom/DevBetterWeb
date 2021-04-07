using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public class MemberRemoveBook
  {
    [Fact]
    public void ShouldDoNothingGivenBookNotInBooksRead()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      Book book = BookHelpers.GetDefaultTestBook();

      member.RemoveBookRead(book);

      // if we get this far, no error was thrown
      Assert.Empty(member.BooksRead);
    }

    [Fact]
    public void ShouldRemoveBookGivenBook()
    {
      Member member = MemberHelpers.CreateWithDefaultConstructor();
      Book book = BookHelpers.GetDefaultTestBook();

      member.AddBookRead(book);

      Assert.Contains(book, member.BooksRead);

      member.RemoveBookRead(book);

      Assert.Empty(member.BooksRead);
    }
  }
}
