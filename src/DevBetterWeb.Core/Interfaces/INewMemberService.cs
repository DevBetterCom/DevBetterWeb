using System.Threading.Tasks;
using Ardalis.Result;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface INewMemberService
{
  Task<Invitation> CreateInvitationAsync(string email, string stripeEventId);
  Task SendRegistrationEmailAsync(Invitation invitation);
  Task<Result<string>> VerifyValidEmailAndInviteCodeAsync(string email, string inviteCode);
  Task<Member> MemberSetupAsync(string userId, string firstName, string lastName, string inviteCode, string email);
}
