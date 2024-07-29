using System.Threading.Tasks;
using DevBetterWeb.Core;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests;

public class RemoveUserFromMemberRoleAsync : MemberSubscriptionCancellationServiceTest
{
  private readonly string _email = "TestEmail";
  private readonly string _userId = "TestId";

  [Fact]
  public async Task RemovesUserFromMemberRole()
  {
	  _userLookup.FindUserIdByEmailAsync(_email).Returns(_userId);

    await _memberCancellationService.RemoveUserFromMemberRoleAsync(_email);

    await _userRoleMembershipService.Received(1).RemoveUserFromRoleByRoleNameAsync(_userId, Constants.MEMBER_ROLE_NAME);
  }
}
