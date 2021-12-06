using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class InactiveInvitationByEmailSpec : Specification<Invitation>, ISingleResultSpecification
{
  public InactiveInvitationByEmailSpec(string email)
  {
    // just return the most recent if multiple
    Query.Where(invite => invite.Email == email)
      .Where(invite => !invite.Active)
      .OrderByDescending(invite => invite.Id)
      .Take(1);
  }
}
