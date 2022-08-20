using System.Collections.Generic;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Pages.Admin.Books;

namespace DevBetterWeb.Web.Pages.User;

public class UserBooksAddModel
{
  public List<Book> BooksAdd { get; set; } = new List<Book>();
  public BookViewModel? AddedBook { get; set; }
  public int? RemovedBook { get; set; }

  public UserBooksAddModel()
  {
  }

  public UserBooksAddModel(Member member)
  {
		BooksAdd = member.UploadedBooks!;
  }

  public bool HasAddBook(Book book)
  {
    foreach (Book bookToCheck in BooksAdd)
    {
      if (bookToCheck.Equals(book))
      {
        return true;
      }
    }

    return false;
  }
}
