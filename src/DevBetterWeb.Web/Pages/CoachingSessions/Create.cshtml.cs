using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.CoachingSessions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class CreateModel : PageModel
{
	public CreateModel()
  {
  }

  public IActionResult OnGet()
  {
    return Page();
  }
}
