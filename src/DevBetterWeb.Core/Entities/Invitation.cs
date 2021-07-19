using System;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
  public class Invitation : BaseEntity, IAggregateRoot
  {
    public string Email { get; private set; }
    public string InviteCode { get; private set; }
    public string PaymentHandlerSubscriptionId { get; private set; }
    public bool Active { get; private set; } = true;
    public DateTime DateCreated { get; private set; }
    public DateTime DateOfUserPing { get; private set; } = DateTime.MinValue;
    public DateTime DateOfLastAdminPing { get; private set; } = DateTime.MinValue;

    public Invitation(string email, string inviteCode, string paymentHandlerSubscriptionId)
    {
      Email = email;
      InviteCode = inviteCode;
      PaymentHandlerSubscriptionId = paymentHandlerSubscriptionId;
      DateCreated = DateTime.Today;
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

    public void UpdateUserPingDate()
    {
      DateOfUserPing = DateTime.Today;
    }

    public void UpdateAdminPingDate()
    {
      DateOfLastAdminPing = DateTime.Today;
    }
  }
}
