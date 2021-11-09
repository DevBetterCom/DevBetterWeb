using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User.MyProfile;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class CancelModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;

  public string Message = "If you are sure you want to unsubscribe from DevBetter, click below.";

  public CancelModel(UserManager<ApplicationUser> userManager,
    IPaymentHandlerSubscription paymentHandlerSubscription)
  {
    _userManager = userManager;
    _paymentHandlerSubscription = paymentHandlerSubscription;
  }

  public async Task<PageResult> OnPost()
  {
    var user = await _userManager.GetUserAsync(User);

    var email = user.Email;

    try
    {
      await _paymentHandlerSubscription.CancelSubscriptionAtPeriodEnd(email);
      Message = "You have been unsubscribed from DevBetter. You will retain access until the end of your subscription period.";
      return Page();
    }
    catch
    {
      Message = "Attempt to cancel subscription failed.";
      return Page();
    }
  }
}
