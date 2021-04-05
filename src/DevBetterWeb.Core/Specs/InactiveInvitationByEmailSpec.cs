using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
  public class InactiveInvitationByEmailSpec : Specification<Invitation>
  {
    public InactiveInvitationByEmailSpec(string email)
    {
      Query.Where(invite => invite.Email == email)
        .Where(invite => !invite.Active);
    }
  }
}
