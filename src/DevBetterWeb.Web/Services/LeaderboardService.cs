using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.Leaderboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Services;

public class LeaderboardService : ILeaderboardService
{
	private readonly IBookService _bookService;
	private readonly IRankAndOrderService _rankAndOrderService;
	private readonly IBookCategoryService _bookCategoryService;
	private readonly IFilteredLeaderboardService _filteredLeaderboardService;

	public LeaderboardService(IRankAndOrderService rankAndOrderService,
		IBookCategoryService bookCategoryService,
		IBookService bookService,
		IFilteredLeaderboardService filteredLeaderboardService)
	{
		_rankAndOrderService = rankAndOrderService;
		_bookCategoryService = bookCategoryService;
		_bookService = bookService;
		_filteredLeaderboardService = filteredLeaderboardService;
	}

	public async Task<List<BookCategoryDto>> SetBookCategoriesAsync()
	{
		var bookCategories = await _bookCategoryService.GetBookCategoriesAsync();

		bookCategories = await _filteredLeaderboardService.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories);

		await _rankAndOrderService.UpdateRanksAndReadBooksCountForMemberAsync(bookCategories);
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);
		_rankAndOrderService.UpdateBooksRank(bookCategories);
		_rankAndOrderService.OrderByRankForMembersAndBooks(bookCategories);

		return bookCategories;
	}

	public async Task<BookDetailsViewModel?> GetBookDetailsAsync(string bookId)
	{
		var bookDetails = await _bookService.GetBookByIdAsync(int.Parse(bookId));
		if (bookDetails == null)
		{
			return null;
		}
		bookDetails = await _filteredLeaderboardService.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetails);

		return bookDetails;
	}
}
