using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;
using System.Linq;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Pages.Leaderboard;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Provides functionality to manage leaderboards with filtered members.
/// </summary>
public class FilteredLeaderboardService : IFilteredLeaderboardService
{
	private readonly INonCurrentMembersService _nonCurrentMembersService;
	private readonly IMemberService _memberService;

	/// <summary>
	/// Initializes a new instance of the <see cref="FilteredLeaderboardService"/> class.
	/// </summary>
	/// <param name="nonCurrentMembersService">The service for managing non-current members.</param>
	public FilteredLeaderboardService(INonCurrentMembersService nonCurrentMembersService, IMemberService memberService)
	{
		_nonCurrentMembersService = nonCurrentMembersService;
		_memberService = memberService;
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

		List<Member> alumniMembers = await _memberService.GetActiveAlumniMembersAsync();
		List<int> alumniMemberIds = alumniMembers.Select(x => x.Id).ToList();

		foreach ( var bookCategoryDto in bookCategories)
		{
			foreach( var book in bookCategoryDto.Books)
			{
				for (int i = book.MembersWhoHaveRead.Count - 1; i >= 0; i--)
				{
					if (nonMembersId.Contains(book.MembersWhoHaveRead[i].Id))
					{
						book.MembersWhoHaveRead.RemoveAt(i);
						book.MembersWhoHaveReadCount--;
						continue;
					}
					if (alumniMemberIds.Count <= 0 || !alumniMemberIds.Contains(book.MembersWhoHaveRead[i].Id))
					{
						book.MembersWhoHaveRead[i].RoleName = AuthConstants.Roles.MEMBERS;						
					}else
					{
						book.MembersWhoHaveRead[i].RoleName = AuthConstants.Roles.ALUMNI;
					}
					book.MembersWhoHaveRead[i].BooksReadCountByCategory = bookCategoryDto.Books.Count(b => b.MembersWhoHaveRead.Exists(m => m.Id == book.MembersWhoHaveRead[i].Id));
				}
			}
			bookCategoryDto.Members = bookCategoryDto.Books.SelectMany(b => b.MembersWhoHaveRead.Where(m => m.RoleName == AuthConstants.Roles.MEMBERS)).Distinct().ToList();
			bookCategoryDto.Alumnus = bookCategoryDto.Books.SelectMany(b => b.MembersWhoHaveRead.Where(m => m.RoleName == AuthConstants.Roles.ALUMNI)).Distinct().ToList();
		}

		return bookCategories;
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
