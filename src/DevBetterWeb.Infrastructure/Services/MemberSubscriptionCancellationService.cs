using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services
{
  public class MemberSubscriptionCancellationService : IMemberCancellationService
  {
    private readonly IUserRoleMembershipService _userRoleMembershipService;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository _repository;
    private readonly MemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public MemberSubscriptionCancellationService(
      IUserRoleMembershipService userRoleMembershipService,
      IEmailService emailService,
      UserManager<ApplicationUser> userManager,
      IRepository repository,
      MemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService)
    {
      _userRoleMembershipService = userRoleMembershipService;
      _emailService = emailService;
      _userManager = userManager;
      _repository = repository;
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
    }

    public async Task RemoveUserFromMemberRoleAsync(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);

      await _userRoleMembershipService.RemoveUserFromRoleByRoleNameAsync(user.Id, Constants.MEMBER_ROLE_NAME);
    }

    public async Task SendFutureCancellationEmailAsync(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);

      var spec = new MemberByUserIdSpec(user.Id);
      var member = await _repository.GetAsync(spec);

      var endDate = _memberSubscriptionPeriodCalculationsService.GetGraduationDate(member);

      var subject = "DevBetter Cancellation";
      var message = $"You have cancelled your DevBetter subscription. Your cancellation will take effect at the end of your current subscription period, on {endDate.ToLongDateString()}.";

      await _emailService.SendEmailAsync(email, subject, message);
    }

    public async Task SendCancellationEmailAsync(string email)
    {

      var subject = "DevBetter Cancellation";
      var message = "Your DevBetter subscription has ended. Thank you for being a part of our community. We hope we'll be able to welcome you back sometime in the future! Don't forget that you can rejoin anytime from https://devbetter.com.";

      await _emailService.SendEmailAsync(email, subject, message);
    }

  }
}
