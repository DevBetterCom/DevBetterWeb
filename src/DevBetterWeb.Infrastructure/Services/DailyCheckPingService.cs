using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using System.Linq;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace DevBetterWeb.Infrastructure.Services
{
  public class DailyCheckPingService : IDailyCheckPingService
  {
    public IRepository<Invitation> _repository;
    public IEmailService _emailService;
    public UserManager<ApplicationUser> _userManager;

    public DailyCheckPingService(IRepository<Invitation> repository,
      IEmailService emailService,
      UserManager<ApplicationUser> userManager)
    {
      _repository = repository;
      _emailService = emailService;
      _userManager = userManager;
    }

    public async Task SendPingIfNeeded(AppendOnlyStringList messages)
    {
      var spec = new ActiveInvitationsSpec();
      var activeInvitations = await _repository.ListAsync(spec);

      var invitationsForUserPing = CheckIfAnyActiveInvitationsRequireUserPing(activeInvitations);
      if (invitationsForUserPing.Any())
      {
        foreach (var invitation in invitationsForUserPing)
        {
          messages.Append($"User at email {invitation.Email} should be reminded to finish setting up their account.");
        }

        var userPingMessages = await SendUserPing(invitationsForUserPing);
        foreach (var message in userPingMessages)
        {
          messages.Append(message);
        }
      }

      var invitationsForAdminPing = CheckIfAnyActiveInvitationsRequireAdminPing(activeInvitations);
      if(invitationsForAdminPing.Any())
      {
        var listOfEmails = "";
        foreach(var invitation in invitationsForAdminPing)
        {
          listOfEmails += $"{invitation.Email}\n";
        }
        messages.Append($"Admins should be reminded to remind these users to finish setting up their DevBetter accounts: {listOfEmails}");

        var adminPingMessage = await SendAdminPing(invitationsForAdminPing);
        messages.Append(adminPingMessage);
      }
    }

    public List<Invitation> CheckIfAnyActiveInvitationsRequireAdminPing(List<Invitation> invitations)
    {
      var invitationsRequiringAdminPing = new List<Invitation>();

      foreach(var invite in invitations)
      {
        if (InviteRequiresAdminPing(invite)) invitationsRequiringAdminPing.Add(invite);
      }

      return invitationsRequiringAdminPing;
    }

    private bool InviteRequiresAdminPing(Invitation invite)
    {
      bool inviteRequiresAdminPing = true;

      if (invite.DateOfUserPing.Equals(DateTime.MinValue)) inviteRequiresAdminPing = false;
      if (invite.DateOfLastAdminPing.Equals(DateTime.Today)) inviteRequiresAdminPing = false;
      if (DateTime.Today - invite.DateCreated < new TimeSpan(4, 0, 0, 0)) inviteRequiresAdminPing = false;
      if (DateTime.Today - invite.DateOfUserPing < new TimeSpan(2, 0, 0, 0)) inviteRequiresAdminPing = false;
      if (!invite.Active) inviteRequiresAdminPing = false;

      return inviteRequiresAdminPing;
    }

    public List<Invitation> CheckIfAnyActiveInvitationsRequireUserPing(List<Invitation> invitations)
    {
      var invitationsRequiringUserPing = new List<Invitation>();

      foreach(var invite in invitations)
      {
        if((DateTime.Today - invite.DateCreated) >= new TimeSpan(2,0,0,0) && invite.DateOfUserPing.Equals(DateTime.MinValue))
        {
          invitationsRequiringUserPing.Add(invite);
        }
      }

      return invitationsRequiringUserPing;
    }

    private async Task<List<string>> SendUserPing(List<Invitation> invitations)
    {
      var messagesToAdd = new List<string>();

      var emailSubject = "Finish setting up your DevBetter account!";
      var emailBody = "Don't forget to click the link below to finish setting up your DevBetterAccount!\n";

      foreach (var invitation in invitations)
      {
        var url = $"https://devbetter.com/Identity/Account/NewMemberRegister/{invitation.InviteCode}/{invitation.Email}";

        await _emailService.SendEmailAsync(invitation.Email, emailSubject, $"{emailBody}{url}");
        messagesToAdd.Add($"User at email {invitation.Email} has been reminded to finish setting up their account.");
        invitation.UpdateUserPingDate();
      }

      return messagesToAdd;
    }
    private async Task<string> SendAdminPing(List<Invitation> invitations)
    {
      var emailSubject = "Remind user(s) to finish setting up their DevBetter account(s)";
      var listOfEmails = "";

      foreach (var invitation in invitations)
      {
        listOfEmails += $"{invitation.Email}\n";
      }

      var emailBody = $"Please remind these users to finish setting up their DevBetter accounts: {listOfEmails}";

      var usersInAdminRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS);

      var emailList = "";

      foreach(var user in usersInAdminRole)
      {
        await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        emailList += $"{user.Email}\n";
      }

      var message = $"Admins were reminded to remind these users to finish setting up their DevBetter accounts: {emailList}";

      foreach(var invitation in invitations)
      {
        invitation.UpdateAdminPingDate();
      }

      return message;

    }
  }
}
