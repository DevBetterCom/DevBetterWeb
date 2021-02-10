using DevBetterWeb.Core.Entities;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface INewMemberService
  {
    Task<Invitation> CreateInvitation(string email, string stripeEventId);
    Task SendRegistrationEmail(Invitation invitation);
    Task<string> VerifyValidEmailAndInviteCode(string email, string inviteCode);
    Task MemberSetup(string userId, string firstName, string lastName, string inviteCode);
    //Task<string> RegisterAspNetUser();
    //Member CreateNewMember(string userId, string firstName, string lastName);
    //Task AddUserToMemberRole(string userId);
    //Subscription CreateSubscriptionForMember(int memberId, int subscriptionLengthInDays);
  }
}
