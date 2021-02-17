using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
  public class Invitation : BaseEntity
  {
    public string Email { get; set; }
    public string InviteCode { get; set; }
    public string PaymentHandlerSubscriptionId { get; set; }
    public bool Active { get; set; } = true;

    public Invitation(string email, string inviteCode, string paymentHandlerSubscriptionId)
    {
      Email = email;
      InviteCode = inviteCode;
      PaymentHandlerSubscriptionId = paymentHandlerSubscriptionId;
    }

    public void Deactivate()
    {
      Active = false;
    }
  }
}
