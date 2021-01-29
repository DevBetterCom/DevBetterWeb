using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Checkout
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

  public class SuccessModel : PageModel
  {
    public void OnGet()
    {
      // membership flow starts!

      // create invite entity

      // send email via NewMemberEmailService
    }
  }
}
