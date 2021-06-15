using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Core.Services
{
  public class MemberSubscriptionCancellationService : IMemberCancellationService
  {
    private readonly IUserRoleMembershipService _userRoleMembershipService;
    private readonly IEmailService _emailService;
    private readonly IUserLookupService _userLookup;
    private readonly IMemberLookupService _memberLookupService;
    private readonly IRepository<Member> _memberRepository;
    private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public MemberSubscriptionCancellationService(
      IUserRoleMembershipService userRoleMembershipService,
      IEmailService emailService,
      IUserLookupService userLookup,
      IMemberLookupService memberLookup,
      IRepository<Member> memberRepository,
      IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService)
    {
      _userRoleMembershipService = userRoleMembershipService;
      _emailService = emailService;
      _userLookup = userLookup;
      _memberLookupService = memberLookup;
      _memberRepository = memberRepository;
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
    }

    public async Task RemoveUserFromMemberRoleAsync(string email)
    {
      var userId = await _userLookup.FindUserIdByEmailAsync(email);

      await _userRoleMembershipService.RemoveUserFromRoleByRoleNameAsync(userId, Constants.MEMBER_ROLE_NAME);
    }

    public async Task SendFutureCancellationEmailAsync(string email)
    {
      var member = await _memberLookupService.GetMemberByEmailAsync(email);

      var endDate = _memberSubscriptionPeriodCalculationsService.GetCurrentSubscriptionEndDate(member);

      var subject = "DevBetter Cancellation";
      var message = $"You have cancelled your DevBetter subscription. Your cancellation will take effect at the end of your current subscription period, on {endDate.ToLongDateString()}.";

      await _emailService.SendEmailAsync(email, subject, message);
    }

    public Task SendCancellationEmailAsync(string email)
    {
      var subject = "DevBetter Cancellation";
      var message = "Your DevBetter subscription has ended. Thank you for being a part of our community. We hope we'll be able to welcome you back sometime in the future! Don't forget that you can rejoin anytime from https://devbetter.com.";

      return _emailService.SendEmailAsync(email, subject, message);
    }
  }
}
