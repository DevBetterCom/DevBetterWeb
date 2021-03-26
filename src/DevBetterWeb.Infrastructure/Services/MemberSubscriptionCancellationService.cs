using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services
{
  public class MemberSubscriptionCancellationService : IMemberCancellationService
  {
    private readonly IUserRoleMembershipService _userRoleMembershipService;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public MemberSubscriptionCancellationService(
      IUserRoleMembershipService userRoleMembershipService,
      IEmailService emailService,
      UserManager<ApplicationUser> userManager)
    {
      _userRoleMembershipService = userRoleMembershipService;
      _emailService = emailService;
      _userManager = userManager;
    }

    public async Task RemoveMemberFromRoleAsync(string userId)
    {
      await _userRoleMembershipService.RemoveUserFromRoleByRoleNameAsync(userId, Constants.MEMBER_ROLE_NAME);
    }

    public async Task SendCancellationEmailAsync(string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);

      var email = user.Email;
      var subject = "DevBetter Cancellation";
      var message = "You have cancelled your DevBetter subscription. Thank you for being a part of our community. We hope we'll be able to welcome you back sometime in the future! Don't forget that you can rejoin anytime from https://devbetter.com.";

      await _emailService.SendEmailAsync(email, subject, message);
    }
  }
}
