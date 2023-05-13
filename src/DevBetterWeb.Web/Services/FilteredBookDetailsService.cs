using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Pages.Leaderboard;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Provides functionality to manage book details with filtered members.
/// </summary>
public class FilteredBookDetailsService : IFilteredBookDetailsService
{
	private readonly INonCurrentMembersService _nonCurrentMembersService;
	private readonly IBookService _bookService;

	/// <summary>
	/// Initializes a new instance of the <see cref="FilteredBookDetailsService"/> class.
	/// </summary>
	/// <param name="nonCurrentMembersService">The service for managing non-current members.</param>
	/// <param name="bookService">The service for get book details.</param>
	public FilteredBookDetailsService(INonCurrentMembersService nonCurrentMembersService, IBookService bookService)
	{
		_nonCurrentMembersService = nonCurrentMembersService;
		_bookService = bookService;
	}

	/// <summary>
	/// Removes non-current members from the book details view model.
	/// </summary>
	/// <param name="bookDetailsViewModel">A book in the book details.</param>
	/// <param name="cancellationToken">An optional token to cancel the operation.</param>
	/// <returns>A book details with non-current members removed.</returns>
	public async Task<BookDetailsViewModel> RemoveNonCurrentMembersFromBookDetailsAsync(BookDetailsViewModel bookDetailsViewModel, CancellationToken cancellationToken = default)
	{
		var nonUsersId = await _nonCurrentMembersService.GetUsersIdsWithoutRolesAsync();
		var nonMembersId = await _nonCurrentMembersService.GetNonCurrentMembersAsync(nonUsersId, cancellationToken);

		bookDetailsViewModel.MembersWhoHaveRead = bookDetailsViewModel.MembersWhoHaveRead
			.Where(member => !nonMembersId.Contains(member.Id))
			.ToList();

		return bookDetailsViewModel;
	}

	/// <summary>
	/// Asynchronously gets the details of a book and removes non-current members from the result.
	/// </summary>
	/// <param name="bookId">The unique identifier of the book to retrieve details for, represented as a string.</param>
	/// <returns>
	/// A <see cref="Task"/> that represents the asynchronous operation. The task result contains a 
	/// <see cref="BookDetailsViewModel"/> that includes the details of the book and the list of members 
	/// who have read the book, excluding non-current members. If the book does not exist, the task result is <c>null</c>.
	/// </returns>
	public async Task<BookDetailsViewModel?> GetBookDetailsAsync(string bookId)
	{
		var bookDetails = await _bookService.GetBookByIdAsync(int.Parse(bookId));
		if (bookDetails == null)
		{
			return null;
		}
		bookDetails = await RemoveNonCurrentMembersFromBookDetailsAsync(bookDetails);

		return bookDetails;
	}
}
