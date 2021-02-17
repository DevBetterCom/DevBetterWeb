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

      var message = "Thank you for joining DevBetter! Please click to complete your registration: " + completeRegistrationUrl + "\n";

      await _emailService.SendEmailAsync(inviteEmail, "Welcome to DevBetter!", message);
    }

    public async Task<string> VerifyValidEmailAndInviteCodeAsync(string email, string inviteCode)
    {
      var spec = new InvitationByInviteCodeWithEmailSpec(inviteCode);

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
        if (storedInviteCode.Email == null)
        {
          throw new InvalidEmailException();
        }
      }
      catch(Exception e)
      {
        ValidEmailAndInviteCode = "Invalid email or invite code: " + e.GetType().ToString();
      }

      return ValidEmailAndInviteCode;
    }

    public async Task<Member> MemberSetupAsync(string userId, string firstName, string lastName, string inviteCode)
    {
      Member member = CreateNewMember(userId, firstName, lastName);
      int memberId = member.Id;
      await AddUserToMemberRoleAsync(userId);

      var spec = new InvitationByInviteCodeWithSubscriptionIdSpec(inviteCode);

      var invite = await _repository.GetAsync(spec);
      var subscriptionId = invite.PaymentHandlerSubscriptionId;

      var subscriptionStart = _paymentHandlerSubscription.GetStartDate(subscriptionId);
      var subscriptionEnd = _paymentHandlerSubscription.GetEndDate(subscriptionId);

      CreateSubscriptionForMember(memberId, subscriptionStart, subscriptionEnd);

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
      var roleName = "Members";

      await _userRoleMembershipService.AddUserToRoleByRoleNameAsync(userId, roleName);

    }

    private Subscription CreateSubscriptionForMember(int memberId, DateTime subscriptionStart, DateTime subscriptionEnd)
    {
      var subscription = new Subscription();
      subscription.MemberId = memberId;
      DateTimeRange dates = new DateTimeRange(subscriptionStart, subscriptionEnd);
      subscription.Dates = dates;
      return subscription;
    }

    private string GetRegistrationUrl(string inviteCode, string inviteEmail)
    {
      var url = $"https://devbetter.com/Identity/Account/NewMemberRegister/{inviteCode}/{inviteEmail}";

      return url;
    }

  }
}
