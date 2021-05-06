using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.UserReports
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
