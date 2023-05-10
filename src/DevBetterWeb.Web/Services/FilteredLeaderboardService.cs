using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;
using System.Linq;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Pages.Leaderboard;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Provides functionality to manage leaderboards with filtered members.
/// </summary>
public class FilteredLeaderboardService : IFilteredLeaderboardService
{
	private readonly INonCurrentMembersService _nonCurrentMembersService;

	/// <summary>
	/// Initializes a new instance of the <see cref="FilteredLeaderboardService"/> class.
	/// </summary>
	/// <param name="nonCurrentMembersService">The service for managing non-current members.</param>
	public FilteredLeaderboardService(INonCurrentMembersService nonCurrentMembersService)
	{
		_nonCurrentMembersService = nonCurrentMembersService;
	}

	/// <summary>
	/// Removes non-current members from the leaderboard.
	/// </summary>
	/// <param name="bookCategories">The list of book categories in the leaderboard.</param>
	/// <param name="cancellationToken">An optional token to cancel the operation.</param>
	/// <returns>A new list of book categories with non-current members removed.</returns>
	public async Task<List<BookCategoryDto>> RemoveNonCurrentMembersFromLeaderBoardAsync(List<BookCategoryDto> bookCategories, CancellationToken cancellationToken = default)
	{
		if (bookCategories.Count <= 0)
		{
			return bookCategories;
		}
		var nonUsersId = await _nonCurrentMembersService.GetUsersIdsWithoutRolesAsync();
		var nonMembersId = await _nonCurrentMembersService.GetNonCurrentMembersAsync(nonUsersId, cancellationToken);

		return bookCategories
				.Select(bookCategory => CreateBookCategoryDtoWithoutNonMembers(bookCategory, nonMembersId))
				.ToList();
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

	private BookCategoryDto CreateBookCategoryDtoWithoutNonMembers(BookCategoryDto bookCategory, List<int> nonMembersId)
	{
		var filteredBooks = CreateBookDtosWithoutNonMembers(bookCategory.Books!, nonMembersId);

		var membersFromBooks = filteredBooks
			.SelectMany(book => book.MembersWhoHaveRead)
			.Distinct()
			.ToList();

		var bookCategoryToAdd = new BookCategoryDto
		{
			Id = bookCategory.Id,
			Title = bookCategory.Title,
			Members = membersFromBooks,
			Books = filteredBooks
		};

		return bookCategoryToAdd;
	}

	private List<MemberForBookDto> RemoveNonMembersFromList(List<MemberForBookDto> members, List<int> nonMembersId)
	{
		return members
				.Where(member => !nonMembersId.Contains(member.Id))
				.ToList();
	}

	private List<BookDto> CreateBookDtosWithoutNonMembers(List<BookDto> books, List<int> nonMembersId)
	{
		return books
				.Select(book => CreateBookDtoWithoutNonMembers(book, nonMembersId))
				.ToList();
	}

	private BookDto CreateBookDtoWithoutNonMembers(BookDto book, List<int> nonMembersId)
	{
		var bookToAdd = new BookDto
		{
			Id = book.Id,
			Title = book.Title,
			Author = book.Author,
			BookCategoryId = book.BookCategoryId,
			CategoryTitle = book.CategoryTitle,
			Details = book.Details,
			MemberWhoUploaded = book.MemberWhoUploaded,
			MemberWhoUploadedUserId = book.MemberWhoUploadedUserId,
			PurchaseUrl = book.PurchaseUrl,
			TitleWithAuthor = book.TitleWithAuthor,
			MembersWhoHaveRead = RemoveNonMembersFromList(book.MembersWhoHaveRead, nonMembersId)
		};

		return bookToAdd;
	}
}
