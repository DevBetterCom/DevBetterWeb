using System.Threading.Tasks;
using Xunit;
using Moq;
using DevBetterWeb.Core;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests
{
  public class RemoveUserFromMemberRoleAsync : MemberSubscriptionCancellationServiceTest
  {
    private readonly string _email = "TestEmail";
    private readonly string _userId = "TestId";

    [Fact]
    public async Task RemovesUserFromMemberRole()
    {
      _userLookup.Setup(u => u.FindUserIdByEmailAsync(_email)).ReturnsAsync(_userId);

      await _memberCancellationService.RemoveUserFromMemberRoleAsync(_email);

      _userRoleMembershipService.Verify(u => u.RemoveUserFromRoleByRoleNameAsync(_userId, Constants.MEMBER_ROLE_NAME), Times.Once);
    }
  }
}


