using DevBetterWeb.Web;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public class ValidDiscordUsernameAttributeHelpersHasHashtagFourFromEndShould
  {
    [Fact]
    public void GiveFalseIfNoHashtagInString()
    {
      var input = "discord";

      var output = ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
      Assert.False(output);
    }

    [Fact]
    public void GiveFalseIfHashtag3FromEnd()
    {
      var input = "discord#www";

      var output = ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
      Assert.False(output);
    }

    [Fact]
    public void GiveFalseIfHashtag5FromEnd()
    {
      var input = "discord#wwttw";

      var output = ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
      Assert.False(output);
    }

    [Fact]
    public void GiveTrueIfHashtag4FromEnd()
    {
      var input = "discord#wwtw";

      var output = ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
      Assert.True(output);
    }

  }
}
