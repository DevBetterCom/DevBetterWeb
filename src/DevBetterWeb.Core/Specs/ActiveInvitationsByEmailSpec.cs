using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
  public class ActiveInvitationByEmailSpec : Specification<Invitation>
  {
    public ActiveInvitationByEmailSpec(string email)
    {
      // just return the most recent if multiple
      Query.Where(invite => invite.Email == email)
        .Where(invite => invite.Active);
    }
  }
}