using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
  public class InvitationByInviteCodeWithSubscriptionIdSpec : Specification<Invitation>
  {
    public InvitationByInviteCodeWithSubscriptionIdSpec(string inviteCode)
    {
      Query.Where(invite => invite.InviteCode == inviteCode);
      Query.Include(invite => invite.PaymentHandlerSubscriptionId);
    }
  }
}
