using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Services;

public class LeaderboardService : ILeaderboardService
{
	private readonly IRankAndOrderService _rankAndOrderService;
	private readonly IBookCategoryService _bookCategoryService;
	private readonly IFilteredLeaderboardService _filteredLeaderboardService;

	public LeaderboardService(IRankAndOrderService rankAndOrderService,
		IBookCategoryService bookCategoryService,
		IFilteredLeaderboardService filteredLeaderboardService)
	{
		_rankAndOrderService = rankAndOrderService;
		_bookCategoryService = bookCategoryService;
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
}
