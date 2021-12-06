using System;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Tests.Services.DailyCheckPingServiceTests;

public class InvitationBuilder
{
  public const string DEFAULT_EMAIL = "test@email.com";
  public const string DEFAULT_INVITE_CODE = "defaultInviteCode";
  public const string DEFAULT_SUBSCRIPTION_ID = "testSubscriptionId";

  private Invitation _invitation = new Invitation(DEFAULT_EMAIL, DEFAULT_INVITE_CODE, DEFAULT_SUBSCRIPTION_ID);

  public InvitationBuilder WithDateCreatedGivenDaysAgo(int daysAgo)
  {
    _invitation.SetPrivateDateTimePropertyValue("DateCreated", DateTime.Today.AddDays(daysAgo * -1));
    return this;
  }
  public InvitationBuilder WithDateOfUserPingGivenDaysAgo(int daysAgo)
  {
    _invitation.SetPrivateDateTimePropertyValue("DateOfUserPing", DateTime.Today.AddDays(daysAgo * -1));
    return this;
  }
  public InvitationBuilder WithDateOfLastAdminPingGivenDaysAgo(int daysAgo)
  {
    _invitation.SetPrivateDateTimePropertyValue("DateOfLastAdminPing", DateTime.Today.AddDays(daysAgo * -1));
    return this;
  }

  public InvitationBuilder AndDeactivated()
  {
    _invitation.Deactivate();
    return this;
  }

  public Invitation Build()
  {
    return _invitation;
  }
}
