using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using System;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Services
{
  public class NewMemberService : INewMemberService
  {

    private readonly IRepository _repository;
    private readonly IUserRoleMembershipService _userRoleMembershipService;

    public NewMemberService(IRepository repository, IUserRoleMembershipService userRoleMembershipService)
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
      if(storedInviteCode == null)
      {
        throw new InvitationNotFoundException();
      }
      if(storedInviteCode.Email == null)
      {
        throw new InvalidEmailException();
      }
    }

    public Task<string> RegisterAspNetUser()
    {
      throw new System.NotImplementedException();
    }

    public Task<Member> CreateNewMember(string userId)
    {
      throw new System.NotImplementedException();
    }

    public Task AddUserToMemberRole(string userId)
    {
      throw new System.NotImplementedException();
    }

    public Task<Subscription> CreateSubscriptionForMember()
    {
      throw new System.NotImplementedException();
    }

  }
}
