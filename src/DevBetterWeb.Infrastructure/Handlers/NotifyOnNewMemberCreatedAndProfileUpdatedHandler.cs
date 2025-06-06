﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Handlers;

public class NotifyOnNewMemberCreatedAndProfileUpdatedHandler : IHandle<NewMemberCreatedAndProfileUpdatedEvent>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailService _emailService;

  public NotifyOnNewMemberCreatedAndProfileUpdatedHandler(UserManager<ApplicationUser> userManager,
      IEmailService emailService)
  {
    _userManager = userManager;
    _emailService = emailService;
  }

  public async Task Handle(NewMemberCreatedAndProfileUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    var usersInAdminRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS);

    var newMemberUser = await _userManager.FindByIdAsync(domainEvent.Member.UserId);
		if (newMemberUser is null) throw new UserNotFoundException(domainEvent.Member.UserId);
    string? newMemberEmail = await _userManager.GetEmailAsync(newMemberUser!);

    foreach (var emailAddress in usersInAdminRole.Select(user => user.Email))
    {
      string subject = $"[devBetter] New Member {domainEvent.Member.UserFullName()}";
      string message = $"A new Member with id {domainEvent.Member.UserId} has signed up and updated their membership profile.\n" +
        $"Member email: {newMemberEmail} \n" +
        $"Member subscription(s): ";
      foreach (var subscription in domainEvent.Member.MemberSubscriptions)
      {
        message = message + $"\n    {subscription.Dates.StartDate} to {subscription.Dates.EndDate}";
      }
      message = message + $"\nView {domainEvent.Member.FirstName}'s profile at: https://devbetter.com/User/Details/{domainEvent.Member.UserId}";
      await _emailService.SendEmailAsync(emailAddress!, subject, message);
    }
  }
}
