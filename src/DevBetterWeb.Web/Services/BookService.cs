using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Pages.Books;
using DevBetterWeb.Web.Pages.Leaderboard;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Service for handling operations related to book categories.
/// </summary>
public class BookService : IBookService
{
	private readonly IRepository<Book> _bookRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="BookService"/> class.
	/// </summary>
	/// <param name="bookRepository">The book category repository.</param>
	public BookService(IRepository<Book> bookRepository)
	{
		_bookRepository = bookRepository;
	}

	/// <summary>
	/// Asynchronously retrieves a book with the specified ID from the repository, including its associated members, 
	/// and maps it to a BookDetailsViewModel.
	/// </summary>
	/// <param name="bookId">The ID of the book to retrieve.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a BookDetailsViewModel representing the book if found, 
	/// or null if no book with the specified ID could be found.
	/// </returns>
	public async Task<BookDetailsViewModel?> GetBookByIdAsync(int bookId)
	{
		var spec = new BookByIdWithMembersSpec(bookId);
		var book = await _bookRepository.FirstOrDefaultAsync(spec);
		if (book == null) return null;

		var bookDetailsViewModel = new BookDetailsViewModel(book);

		return bookDetailsViewModel;
	}

	/// <summary>
	/// Asynchronously retrieves all books from the repository, ordered by the BooksOrderedByReadsSpec specification, 
	/// and maps them into a list of BookViewModels.
	/// </summary>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a list of BookViewModels.
	/// </returns>
	public async Task<List<BookViewModel>> GetAllBooksAsync()
	{
		var spec = new BooksOrderedByReadsSpec();
		var books = await _bookRepository.ListAsync(spec);

		var booksViewModel = BookViewModel.FromBooks(books);

		return booksViewModel
			.OrderBy(b => b.BookCategoryId)
			.ThenByDescending(b => b.ReadsCount)
			.ToList();
	}
}
