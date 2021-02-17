using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
  public class Invitation : BaseEntity
  {
    public string Email { get; private set; }
    public string InviteCode { get; private set; }
    public string PaymentHandlerSubscriptionId { get; private set; }
    public bool Active { get; private set; } = true;

    public Invitation(string email, string inviteCode, string paymentHandlerSubscriptionId)
    {
      Email = email;
      InviteCode = inviteCode;
      PaymentHandlerSubscriptionId = paymentHandlerSubscriptionId;
    }

    private Invitation()
    {
      Email = "";
      InviteCode = "";
      PaymentHandlerSubscriptionId = "";
    }

    public void Deactivate()
    {
      Active = false;
    }
  }
}
