using System.Threading.Tasks;
using DevBetterWeb.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard;

[Authorize]
public class BookDetailsModel : PageModel
{
	private readonly ILeaderboardService _leaderboardService;
	public BookDetailsViewModel? BookDetailsViewModel { get; set; }

	public BookDetailsModel(ILeaderboardService leaderboardService)
  {
	  _leaderboardService = leaderboardService;
  }

  public async Task<IActionResult> OnGet(string bookId)
  {
    BookDetailsViewModel = await _leaderboardService.GetBookDetailsAsync(bookId);

		return Page();
  }
}
