using System;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Tests.Services.DailyCheckPingServiceTests
{
  public static class InvitationBuilder
  {
    public const string DEFAULT_EMAIL = "test@email.com";
    public const string DEFAULT_INVITE_CODE = "defaultInviteCode";
    public const string DEFAULT_SUBSCRIPTION_ID = "testSubscriptionId";

    public static Invitation BuildDefaultInvitation()
    {
      return new Invitation(DEFAULT_EMAIL, DEFAULT_INVITE_CODE, DEFAULT_SUBSCRIPTION_ID);
    }

    public static Invitation WithDateCreatedGivenDaysAgo(this Invitation invite, int daysAgo)
    {
      invite.SetPrivateDateTimePropertyValue("DateCreated", DateTime.Today.AddDays(daysAgo * -1));
      return invite;
    }
    public static Invitation WithDateOfUserPingGivenDaysAgo(this Invitation invite, int daysAgo)
    {
      invite.SetPrivateDateTimePropertyValue("DateOfUserPing", DateTime.Today.AddDays(daysAgo * -1));
      return invite;
    }
    public static Invitation WithDateOfLastAdminPingGivenDaysAgo(this Invitation invite, int daysAgo)
    {
      invite.SetPrivateDateTimePropertyValue("DateOfLastAdminPing", DateTime.Today.AddDays(daysAgo * -1));
      return invite;
    }

    public static Invitation AndDeactivated(this Invitation invite)
    {
      invite.Deactivate();
      return invite;
    }
  }
}
