﻿using Ardalis.Result;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.ValueObjects;
using System;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Services
{
  public class NewMemberService : INewMemberService
  {

    private readonly IRepository _repository;
    private readonly IUserRoleMembershipService _userRoleMembershipService;
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
    private readonly IEmailService _emailService;

    public NewMemberService(IRepository repository,
      IUserRoleMembershipService userRoleMembershipService,
      IPaymentHandlerSubscription paymentHandlerSubscription,
      IEmailService emailService)
    {
      _repository = repository;
      _userRoleMembershipService = userRoleMembershipService;
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _emailService = emailService;
    }

    public async Task<Invitation> CreateInvitationAsync(string email, string stripeSubscriptionId)
    {
      var inviteCode = Guid.NewGuid().ToString();
      var invitation = new Invitation(email, inviteCode, stripeSubscriptionId);

      await _repository.AddAsync(invitation);

      return invitation;
    }

    public async Task SendRegistrationEmailAsync(Invitation invitation)
    {
      string code = invitation.InviteCode;
      string inviteEmail = invitation.Email;

      string completeRegistrationUrl = GetRegistrationUrl(code, inviteEmail);

      // TODO: send Discord invite here as well

      var message = $"Thank you for joining DevBetter! Please click to complete your registration:\n\n {completeRegistrationUrl}\n\nWe're so glad to have you here!";

      await _emailService.SendEmailAsync(inviteEmail, "Welcome to DevBetter!", message);
    }

    public async Task<Result<string>> VerifyValidEmailAndInviteCodeAsync(string email, string inviteCode)
    {
      var spec = new InvitationByInviteCodeSpec(inviteCode);

      string ValidEmailAndInviteCode = "success";

      try
      {
        var storedInviteCode = await _repository.GetAsync(spec);
        if (storedInviteCode == null)
        {
          throw new InvitationNotFoundException();
        }
        if (!storedInviteCode.Active)
        {
          throw new InvitationNotActiveException();
        }
        if (string.IsNullOrEmpty(storedInviteCode.Email))
        {
          throw new InvalidEmailException();
        }
      }
      catch(Exception e)
      {
        ValidEmailAndInviteCode = "Invalid email or invite code: " + e.GetType().ToString();
      }

      return Result<string>.Success(ValidEmailAndInviteCode);
    }

    public async Task<Member> MemberSetupAsync(string userId, string firstName, string lastName, string inviteCode)
    {
      Member member = CreateNewMember(userId, firstName, lastName);
      await AddUserToMemberRoleAsync(userId);

      var spec = new InvitationByInviteCodeSpec(inviteCode);

      var invite = await _repository.GetAsync(spec);
      var subscriptionId = invite.PaymentHandlerSubscriptionId;

      var subscriptionDateTimeRange = _paymentHandlerSubscription.GetDateTimeRange(subscriptionId);

      CreateSubscriptionForMember(member.Id, subscriptionDateTimeRange);

      // Member has now been created and set up from the invite used. Invite should now be deactivated
      invite.Deactivate();

      return member;
    }


    private Member CreateNewMember(string userId, string firstName, string lastName)
    {
      Member member = new Member(userId);
      member.UpdateName(firstName, lastName);

      return member;
    }

    private async Task AddUserToMemberRoleAsync(string userId)
    {
      var roleName = Constants.MEMBER_ROLE_NAME;

      await _userRoleMembershipService.AddUserToRoleByRoleNameAsync(userId, roleName);

    }

    private Subscription CreateSubscriptionForMember(int memberId, DateTimeRange subscriptionDateTimeRange)
    {
      var subscription = new Subscription();
      subscription.MemberId = memberId;
      subscription.Dates = subscriptionDateTimeRange;
      return subscription;
    }

    private string GetRegistrationUrl(string inviteCode, string inviteEmail)
    {
      var url = $"https://devbetter.com/Identity/Account/NewMemberRegister/{inviteCode}/{inviteEmail}";

      return url;
    }

  }
}
