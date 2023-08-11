using System.Threading.Tasks;
using System.Threading;
using DevBetterWeb.Web.Pages.Leaderboard;

namespace DevBetterWeb.Web.Interfaces;

public interface IFilteredBookDetailsService
{
	/// <summary>
	/// Removes non-current members from the book details view model.
	/// </summary>
	/// <param name="bookDetailsViewModel">A book in the book details.</param>
	/// <param name="cancellationToken">An optional token to cancel the operation.</param>
	/// <returns>A book details with non-current members removed.</returns>
	Task<BookDetailsViewModel> RemoveNonCurrentMembersFromBookDetailsAsync(BookDetailsViewModel bookDetailsViewModel,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously gets the details of a book and removes non-current members from the result.
	/// </summary>
	/// <param name="bookId">The unique identifier of the book to retrieve details for, represented as a string.</param>
	/// <returns>
	/// A <see cref="Task"/> that represents the asynchronous operation. The task result contains a 
	/// <see cref="BookDetailsViewModel"/> that includes the details of the book and the list of members 
	/// who have read the book, excluding non-current members. If the book does not exist, the task result is <c>null</c>.
	/// </returns>
	Task<BookDetailsViewModel?> GetBookDetailsAsync(string bookId);
}
