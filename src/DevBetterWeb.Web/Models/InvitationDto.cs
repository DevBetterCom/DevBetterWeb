using System;

namespace DevBetterWeb.Web.Models;

public class InvitationDto
{
  public string? Email { get; set; }
  public string? InviteCode { get; set; }
  public string? PaymentHandlerSubscriptionId { get; set; }
  public bool Active { get; set; }
  public DateTime DateCreated { get; set; }
  public DateTime? DateOfUserPing { get; set; }
  public DateTime? DateOfLastAdminPing { get; set; }
}
