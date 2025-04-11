using System.Linq;
using DevBetterWeb.Core.Events;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberNew
{
  [Fact]
  public void IsPrivateForDefaultConstructor()
  {
    var member = MemberHelpers.CreateWithDefaultConstructor();

    Assert.NotNull(member);
  }

  [Fact]
  public void CreatesNoEventsWithDefaultConstructor()
  {
    var member = MemberHelpers.CreateWithDefaultConstructor();

    Assert.Empty(member.DomainEvents);
  }

  [Fact]
  public void RequiresUserIdForInternalUsage()
  {
    var member = MemberHelpers.CreateWithInternalConstructor();

    Assert.Equal(MemberHelpers.TEST_USER_ID, member.UserId);
  }

  [Fact]
  public void WithUserIdRecordsNewMemberCreatedEvent()
  {
    var member = MemberHelpers.CreateWithInternalConstructor();

    var eventCreated = (NewMemberCreatedEvent)member.DomainEvents.First();

    Assert.Same(member, eventCreated.Member);
  }
}
