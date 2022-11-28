using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services;

public class MemberSubscriptionEndedAdminEmailService : IMemberSubscriptionEndedAdminEmailService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailService _emailService;
  private readonly IMemberLookupService _memberLookupService;

  public MemberSubscriptionEndedAdminEmailService(UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IMemberLookupService memberLookup)
  {
    _userManager = userManager;
    _emailService = emailService;
    _memberLookupService = memberLookup;
  }

  public async Task SendMemberSubscriptionEndedEmailAsync(string customerEmail, Member? memberFullInfo)
  {
    var subject = "DevBetter Cancellation";

    var member = await _memberLookupService.GetMemberByEmailAsync(customerEmail);
    var memberName = member.UserFullName();

    var message = $"{memberName}'s DevBetter subscription has ended. Please remove {memberName} from the GitHub{GitHubUrl(memberFullInfo)}, Discord{DiscordUsername(memberFullInfo)} and StackOverflow groups{StackOverflowEmail(memberFullInfo)}.";

    var usersInAdminRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS);

    foreach (var emailAddress in usersInAdminRole.Select(user => user.Email))
    {
      await _emailService.SendEmailAsync(emailAddress!, subject, message);
    }
  }

  private string GitHubUrl(Member? memberFullInfo)
  {
	  return memberFullInfo == null ? string.Empty : $" ({memberFullInfo?.GitHubUrl})";
  }

  private string DiscordUsername(Member? memberFullInfo)
  {
	  return memberFullInfo == null ? string.Empty : $" ({memberFullInfo?.DiscordUsername})";
  }

  private string StackOverflowEmail(Member? memberFullInfo)
  {
		//TODO: Not sure same email or not? maybe we need to implement it to be auto with discord and other things.
	  return memberFullInfo == null ? string.Empty : $" ({memberFullInfo?.Email})";
  }
}
