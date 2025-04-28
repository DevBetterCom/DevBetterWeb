using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Handlers;

public class NotifyOnNewMemberCreatedHandler : IHandle<NewMemberCreatedEvent>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailService _emailService;

  public NotifyOnNewMemberCreatedHandler(UserManager<ApplicationUser> userManager,
      IEmailService emailService)
  {
    _userManager = userManager;
    _emailService = emailService;
  }

  public async Task Handle(NewMemberCreatedEvent domainEvent, CancellationToken cancellationToken)
  {
    var usersInAdminRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS);

    foreach (var emailAddress in usersInAdminRole.Select(user => user.Email))
    {
      string subject = $"[devBetter] New Member {domainEvent.Member.UserFullName()}";
      string message = $"A new Member with id {domainEvent.Member.UserId} and email address {domainEvent.Member.Email!} has signed up and added their membership profile." +
				$"\n" +
				$"\nhttps://devbetter.com/Admin/User?userid={domainEvent.Member.UserId}";
      await _emailService.SendEmailAsync(emailAddress!, subject, message);
    }
  }
}
