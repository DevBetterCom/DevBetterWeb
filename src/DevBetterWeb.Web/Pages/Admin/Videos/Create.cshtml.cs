using DevBetterWeb.Core;
using DevBetterWeb.Web.Models.Vimeo;
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

  [BindProperty]
  public VideoModel VideoModel { get; set; } = new VideoModel();
}
