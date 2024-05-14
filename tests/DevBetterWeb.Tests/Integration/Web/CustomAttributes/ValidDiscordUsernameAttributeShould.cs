using DevBetterWeb.Web;
using Xunit;

namespace DevBetterWeb.Tests.Web.CustomAttributes;

public class ValidDiscordUsernameAttributeShould
{
	[Fact]
	public void GiveFalseIfHas1Character()
	{
		var attribute = new ValidDiscordUsernameAttribute();
		
		var input = "a";
		var isValid = attribute.IsValid(input);

		Assert.False(isValid);
	}

	[Fact]
	public void GiveFalseIfHasMoreThan32Characters()
	{
		var attribute = new ValidDiscordUsernameAttribute();

		var input = new string('a', 33);
		var isValid = attribute.IsValid(input);

		Assert.False(isValid);
	}

	[Fact]
	public void GiveFalseIfHasSpecialCharacterBesidesUnderscoreAndPeriod()
	{
		var attribute = new ValidDiscordUsernameAttribute();

		var input = "john!@#$";
		var isValid = attribute.IsValid(input);

		Assert.False(isValid);
	}

	[Fact]
	public void GiveFalseIfHas2ConsecutivePeriodCharacters()
	{
		var attribute = new ValidDiscordUsernameAttribute();

		var input = "john..doe";
		var isValid = attribute.IsValid(input);

		Assert.False(isValid);
	}

	[Theory]
	[InlineData("jo")]
	[InlineData("johndoe")]
	[InlineData("areallylongjohndoe")]
	[InlineData("areallylonglonglongjohndoe")]
	[InlineData("areallylonglonglonglongjohndoe")]
	public void GiveTrueIfHasAtLeast2CharactersAndAtMost32(string input)
	{
		var attribute = new ValidDiscordUsernameAttribute();

		var isValid = attribute.IsValid(input);

		Assert.True(isValid);
	}

	[Theory]
	[InlineData(".johndoe")]
	[InlineData("johndoe123")]
	[InlineData("john.doe.123")]
	[InlineData("john_doe.123")]
	[InlineData("john_doe_123")]
	public void GiveTrueIfHasNumbersUnderscoreOrPeriod(string input)
	{
		var attribute = new ValidDiscordUsernameAttribute();

		var isValid = attribute.IsValid(input);

		Assert.True(isValid);
	}
}
