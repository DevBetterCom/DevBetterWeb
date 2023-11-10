using System.Collections.Generic;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.Books;

public class BookViewModel
{
	public string? Title { get; set; }
	public string? Author { get; set; }
	public string? MemberWhoUploadName { get; set; }
	public string? Details { get; set; }
	public string? PurchaseUrl { get; set; }
	public string? BookCategoryName { get; set; }
	public int ReadsCount { get; set; }
	public int? BookCategoryId { get; set; }

	public BookViewModel()
  {
  }

  public BookViewModel(Book book)
  {
    Title = book.Title;
    Author = book.Author;
		MemberWhoUploadName = book.MemberWhoUpload?.UserFullName();
		Details = book.Details;
    PurchaseUrl = book.PurchaseUrl;
		BookCategoryName = book.BookCategory?.Title;
		ReadsCount = book.MembersWhoHaveRead!.Count;
		BookCategoryId = book.BookCategoryId;
	}

	public static List<BookViewModel> FromBooks(List<Book> books)
	{
		var result = new List<BookViewModel>();
		foreach (var book in books)
		{
			result.Add(new BookViewModel(book));
		}

		return result;
	}
}
