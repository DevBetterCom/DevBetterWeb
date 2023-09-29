using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.Videos;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class CreateModel : PageModel
{

	public IActionResult OnGet()
  {
    return Page();
  }
}
