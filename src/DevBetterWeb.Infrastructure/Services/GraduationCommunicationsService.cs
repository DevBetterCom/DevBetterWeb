using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services
{
  public class GraduationCommunicationsService : IGraduationCommunicationsService
  {
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GraduationCommunicationsService(IEmailService emailService,
      UserManager<ApplicationUser> userManager)
    {
      _emailService = emailService;
      _userManager = userManager;
    }

    public async Task SendGraduationCommunications(Member member)
    {
      var memberEmail = "";
      var memberSubject = "Congratulations from DevBetter!";
      var memberText = "You're now an alumnus of DevBetter! Congratulations on your graduation!\nNow that you're an alumnus, you'll retain full access as long as you want it, but your subscription is free.";
      await _emailService.SendEmailAsync(memberEmail, memberSubject, memberText);

      var adminSubject = "DevBetter Graduation";
      var adminText = $"{member.UserFullName()} is now an alumnus of DevBetter. Please add them to the Alumni role in Discord and ensure that they are no longer paying for access.";

      var usersInAdminRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS);

      foreach (var emailAddress in usersInAdminRole.Select(user => user.Email))
      {
        await _emailService.SendEmailAsync(emailAddress, adminSubject, adminText);
      }
    }
  }
}
