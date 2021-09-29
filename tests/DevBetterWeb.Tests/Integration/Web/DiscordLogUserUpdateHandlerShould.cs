using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Handlers;
using Xunit;

namespace DevBetterWeb.Tests.Integration.Web
{
  public class DiscordLogUserUpdateHandlerShould
  {
    [Fact]
    public void ReturnsProperMessageString()
    {
      Member member = MemberHelpers.CreateWithInternalConstructor();
      member.UpdateName("Steve", "Smith");

      MemberUpdatedEvent memberEvent = new MemberUpdatedEvent(member, "Links");

      var output = DiscordLogMemberUpdateHandler.returnWebhookMessageString(memberEvent);
      var expected = $"User Steve Smith just updated their profile. Check it out here: https://devbetter.com/User/Details/TestUserId.";

      Assert.Equal(expected, output);
    }
  }
}
