using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class ActiveInvitationsSpec : Specification<Invitation>
{
  public ActiveInvitationsSpec()
  {
    Query.Where(invite => invite.Active);
  }
}
