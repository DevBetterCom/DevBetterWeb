using System.Threading.Tasks;
using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Resources
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
  public class IndexModel : PageModel
  {
    public IndexModel()
    {
    }

    public async Task OnGet()
    {
    }
  }
}
