using System.Threading.Tasks;
using DevBetterWeb.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard;

[Authorize]
public class BookDetailsModel : PageModel
{
	private readonly IFilteredBookDetailsService _filteredBookDetailsService;
	public BookDetailsViewModel? BookDetailsViewModel { get; set; }

	public BookDetailsModel(IFilteredBookDetailsService filteredBookDetailsService)
	{
		_filteredBookDetailsService = filteredBookDetailsService;
	}

  public async Task<IActionResult> OnGet(string bookId)
  {
    BookDetailsViewModel = await _filteredBookDetailsService.GetBookDetailsAsync(bookId);

		return Page();
  }
}
