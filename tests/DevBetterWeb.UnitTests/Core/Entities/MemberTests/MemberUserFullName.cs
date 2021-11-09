using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberUserFullName
{
  private const string TEST_FIRST_NAME = "firstname";
  private const string TEST_LAST_NAME = "lastname";

  [Theory]
  [InlineData(null, null)]
  [InlineData("", null)]
  [InlineData(null, "")]
  [InlineData("", "")]
  public void ReturnsNoNameProvidedGivenNullOrEmptyNames(string? firstName, string? lastName)
  {
    var member = MemberHelpers.CreateWithDefaultConstructor();
    member.UpdateName(firstName, lastName);

    Assert.Equal("[No Name Provided]", member.UserFullName());
  }

  [Fact]
  public void ReturnsFirstNameSpaceLastNameWhenBothHaveValues()
  {
    var member = MemberHelpers.CreateWithDefaultConstructor();
    member.UpdateName(TEST_FIRST_NAME, TEST_LAST_NAME);

    Assert.Equal(TEST_FIRST_NAME + " " + TEST_LAST_NAME, member.UserFullName());
  }
}
