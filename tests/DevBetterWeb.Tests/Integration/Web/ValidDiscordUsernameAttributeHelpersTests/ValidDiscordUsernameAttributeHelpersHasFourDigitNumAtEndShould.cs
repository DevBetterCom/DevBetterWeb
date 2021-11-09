using Xunit;

namespace DevBetterWeb.Tests.Web.ValidDiscordUsernameAttributeHelpers;

public class ValidDiscordUsernameAttributeHelpersHasFourDigitNumAtEndShould
{
  [Fact]
  public void GiveFalseIfNoNumInString()
  {
    var input = "discord";

    var output = DevBetterWeb.Web.ValidDiscordUsernameAttributeHelpers.HasFourDigitNumAtEnd(input);

    Assert.False(output);
  }

  [Fact]
  public void GiveFalseIf3DigitNumAtEnd()
  {
    var input = "discord123";

    var output = DevBetterWeb.Web.ValidDiscordUsernameAttributeHelpers.HasFourDigitNumAtEnd(input);

    Assert.False(output);
  }

  [Fact]
  public void GiveTrueGiven4DigitNumAtEnd()
  {
    var input = "discord1234";

    var output = DevBetterWeb.Web.ValidDiscordUsernameAttributeHelpers.HasFourDigitNumAtEnd(input);

    Assert.True(output);

  }
}
