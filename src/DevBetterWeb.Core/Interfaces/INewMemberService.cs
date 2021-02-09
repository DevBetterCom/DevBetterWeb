using DevBetterWeb.Core.Entities;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface INewMemberService
  {
    Task<Invitation> CreateInvitation(string email, string stripeEventId);
    Task SendRegistrationEmail(Invitation invitation);
    Task VerifyValidEmailAndInviteCode(string email, string inviteCode);
    Task<string> RegisterAspNetUser();
    Task<Member> CreateNewMember(string userId);
    Task AddUserToMemberRole(string userId);
    Task<Subscription> CreateSubscriptionForMember();
  }
}
