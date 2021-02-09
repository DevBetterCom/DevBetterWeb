using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
  public class Invitation : BaseEntity
  {
    public string Email { get; set; }
    public string InviteCode { get; set; }
    public string StripeSubscriptionId { get; set; }
    public bool Active { get; set; } = true;

    public Invitation(string email, string inviteCode, string stripeSubscriptionId)
    {
      Email = email;
      InviteCode = inviteCode;
      StripeSubscriptionId = stripeSubscriptionId;
    }

    public void Deactivate()
    {
      Active = false;
    }
  }
}
