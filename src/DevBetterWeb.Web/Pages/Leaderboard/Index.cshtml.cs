using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class IndexModel : PageModel
{
	private readonly IRankAndOrderService _rankAndOrderService;
	private readonly IBookCategoryService _bookCategoryService;
	private readonly IFilteredLeaderboardService _filteredLeaderboardService;

  public List<BookCategoryDto> BookCategories { get; set; } = new ();

  public IndexModel(
	  IRankAndOrderService rankAndOrderService,
		IBookCategoryService bookCategoryService,
	  IFilteredLeaderboardService filteredLeaderboardService)
  {
	  _rankAndOrderService = rankAndOrderService;
	  _bookCategoryService = bookCategoryService;
	  _filteredLeaderboardService = filteredLeaderboardService;
  }

  public async Task OnGet()
  {
	  await SetBookCategoriesAsync();
  }

  private async Task SetBookCategoriesAsync()
  {
	  BookCategories = await _bookCategoryService.GetBookCategoriesAsync();

		BookCategories = await _filteredLeaderboardService.RemoveNonCurrentMembersFromLeaderBoardAsync(BookCategories);

		await _rankAndOrderService.UpdateRanksAndReadBooksCountForMemberAsync(BookCategories);
		_rankAndOrderService.UpdateMembersReadRank(BookCategories);
		_rankAndOrderService.UpdateBooksRank(BookCategories);
	  _rankAndOrderService.OrderByRankForMembersAndBooks(BookCategories);
  }
}
