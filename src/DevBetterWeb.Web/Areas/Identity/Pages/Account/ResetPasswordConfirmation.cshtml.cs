using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResetPasswordConfirmationModel : PageModel
{
  public void OnGet()
  {

  }
}
