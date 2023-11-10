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
	private readonly ILeaderboardService _leaderboardService;

  public List<BookCategoryDto> BookCategories { get; set; } = new ();

  public IndexModel(ILeaderboardService leaderboardService)
  {
	  _leaderboardService = leaderboardService;
  }

  public async Task OnGet()
  {
	  BookCategories = await _leaderboardService.SetBookCategoriesAsync();
  }
}
