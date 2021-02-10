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
      IUserRoleMembershipService userRoleMembershipService
      )
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

    public Task SendRegistrationEmail(Invitation invitation)
    {
      throw new System.NotImplementedException();
    }

    public async Task VerifyValidEmailAndInviteCode(string email, string inviteCode)
    {
      var spec = new InvitationByInviteCodeWithEmailSpec(inviteCode);

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

    public async Task MemberSetup(string firstName, string lastName, int subscriptionLengthInDays)
    {
      string userId = await RegisterAspNetUser();
      Member member = CreateNewMember(userId, firstName, lastName);
      int memberId = member.Id;
      await AddUserToMemberRole(userId);
      CreateSubscriptionForMember(memberId, subscriptionLengthInDays);
    }

    private Task<string> RegisterAspNetUser()
    {
      // Get user id from new user created on NewMemberRegsiter page

      throw new System.NotImplementedException();
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

    private Subscription CreateSubscriptionForMember(int memberId, int subscriptionLengthInDays)
    {
      var subscription = new Subscription();
      subscription.MemberId = memberId;
      DateTime endDate = DateTime.Today.AddDays(subscriptionLengthInDays);
      DateTimeRange dates = new DateTimeRange(DateTime.Today, endDate);
      subscription.Dates = dates;
      return subscription;
    }

  }
}
