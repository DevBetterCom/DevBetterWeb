using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Resources
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  public class IndexModel : PageModel
  {
    public IndexModel()
    {
    }

    public void OnGet()
    {
    }
  }
}
