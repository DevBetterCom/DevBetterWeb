using DevBetterWeb.Core.Entities;
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

    public NewMemberService(IRepository repository,
      IUserRoleMembershipService userRoleMembershipService)
    {
      _repository = repository;
      _userRoleMembershipService = userRoleMembershipService;
    }

    public async Task<Invitation> CreateInvitation(string email, string stripeSubscriptionId)
    {
      var inviteCode = Guid.NewGuid().ToString();
      var invitation = new Invitation(email, inviteCode, stripeSubscriptionId);

      await _repository.AddAsync(invitation);

      return invitation;
    }

    public Task SendRegistrationEmail(Invitation invitation, string callbackUrl)
    {
      string code = invitation.InviteCode;
      string inviteEmail = invitation.Email;

      var callbackUrl = Url.Page(
        "/Account/NewMemberRegister",
        pageHandler: null,
        values: new { inviteCode = code, email = inviteEmail },
        protocol: Request.Scheme);

      throw new System.NotImplementedException();
    }

    public async Task<string> VerifyValidEmailAndInviteCode(string email, string inviteCode)
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

    public async Task MemberSetup(string userId, string firstName, string lastName, string inviteCode)
    {
      Member member = CreateNewMember(userId, firstName, lastName);
      int memberId = member.Id;
      await AddUserToMemberRole(userId);

      var spec = new InvitationByInviteCodeWithSubscriptionIdSpec(inviteCode);

      var invite = await _repository.GetAsync(spec);
      var subscriptionId = invite.StripeSubscriptionId;



      CreateSubscriptionForMember(memberId, subscriptionStart, subscriptionEnd);
    }


    private Member CreateNewMember(string userId, string firstName, string lastName)
    {
      Member member = new Member(userId);
      member.UpdateName(firstName, lastName);

      return member;
    }

    private async Task AddUserToMemberRole(string userId)
    {
      var roleName = "Members";

      await _userRoleMembershipService.AddUserToRoleAsyncByRoleName(userId, roleName);

    }

    private Subscription CreateSubscriptionForMember(int memberId, DateTime subscriptionStart, DateTime subscriptionEnd)
    {
      var subscription = new Subscription();
      subscription.MemberId = memberId;
      DateTimeRange dates = new DateTimeRange(subscriptionStart, subscriptionEnd);
      subscription.Dates = dates;
      return subscription;
    }

  }
}
