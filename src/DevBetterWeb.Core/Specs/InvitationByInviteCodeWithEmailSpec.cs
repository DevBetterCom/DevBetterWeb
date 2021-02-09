using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
  public class InvitationByInviteCodeWithEmailSpec : Specification<Invitation>
  {
    public InvitationByInviteCodeWithEmailSpec(string inviteCode)
    {
      Query.Where(invite => invite.InviteCode == inviteCode);
      Query.Include(invite => invite.Email);
    }
  }
}
