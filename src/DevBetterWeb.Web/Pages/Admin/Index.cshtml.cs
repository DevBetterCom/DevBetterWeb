using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class IndexModel : PageModel
  {
    public IndexModel(IWebHostEnvironment hostingEnvironment)
    {
      HostingEnvironment = hostingEnvironment;
    }

    public IWebHostEnvironment HostingEnvironment { get; }
  }
}
