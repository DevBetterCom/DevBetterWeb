using Xunit;

namespace DevBetterWeb.Tests.Web.ValidDiscordUsernameAttributeHelpers;

public class ValidDiscordUsernameAttributeHelpersHasHashtagFourFromEndShould
{
  [Fact]
  public void GiveFalseIfNoHashtagInString()
  {
    var input = "discord";

    var output = DevBetterWeb.Web.ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
    Assert.False(output);
  }

  [Fact]
  public void GiveFalseIfHashtag3FromEnd()
  {
    var input = "discord#www";

    var output = DevBetterWeb.Web.ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
    Assert.False(output);
  }

  [Fact]
  public void GiveFalseIfHashtag5FromEnd()
  {
    var input = "discord#wwttw";

    var output = DevBetterWeb.Web.ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
    Assert.False(output);
  }

  [Fact]
  public void GiveTrueIfHashtag4FromEnd()
  {
    var input = "discord#wwtw";

    var output = DevBetterWeb.Web.ValidDiscordUsernameAttributeHelpers.HasHashtagFourFromEnd(input);
    Assert.True(output);
  }

}
