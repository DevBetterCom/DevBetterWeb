using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Web.Pages.Books;
using DevBetterWeb.Web.Pages.Leaderboard;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Interface for the BookService.
/// </summary>
public interface IBookService
{
	/// <summary>
	/// Asynchronously retrieves a book with the specified ID from the repository, including its associated members, 
	/// and maps it to a BookDetailsViewModel.
	/// </summary>
	/// <param name="bookId">The ID of the book to retrieve.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a BookDetailsViewModel representing the book if found, 
	/// or null if no book with the specified ID could be found.
	/// </returns>
	Task<BookDetailsViewModel?> GetBookByIdAsync(int bookId);

	/// <summary>
	/// Asynchronously retrieves all books from the repository, ordered by the BooksOrderedByReadsSpec specification, 
	/// and maps them into a list of BookViewModels.
	/// </summary>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a list of BookViewModels.
	/// </returns>
	Task<List<BookViewModel>> GetAllBooksAsync();
}
