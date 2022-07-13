using System;
using System.Linq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberUpdateName
{
  private string _initialFirstName = "";
  private string _initialLastName = "";

  private Member GetMemberWithDefaultName()
  {
    _initialFirstName = Guid.NewGuid().ToString();
    _initialLastName = Guid.NewGuid().ToString();

    var member = MemberHelpers.CreateWithDefaultConstructor();
    member.UpdateName(_initialFirstName, _initialLastName);
    member.Events.Clear();

    return member;
  }

  [Fact]
  public void SetsFirstNameAndLastName()
  {
    string newFirstName = Guid.NewGuid().ToString();
    string newLastName = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultName();
    member.UpdateName(newFirstName, newLastName);

    Assert.Equal(newFirstName, member.FirstName);
    Assert.Equal(newLastName, member.LastName);
  }

  [Fact]
  public void RecordsEventIfFirstNameChanges()
  {
    string newFirstName = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultName();
    member.UpdateName(newFirstName, _initialLastName);
    var eventCreated = (MemberUpdatedEvent)member.Events.First();

    Assert.Same(member, eventCreated.Member);
    Assert.Equal("Name", eventCreated.UpdateDetails);
  }

  [Fact]
  public void RecordsEventIfLastNameChanges()
  {
    string newLastName = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultName();
    member.UpdateName(_initialFirstName, newLastName);
    var eventCreated = (MemberUpdatedEvent)member.Events.First();

    Assert.Same(member, eventCreated.Member);
    Assert.Equal("Name", eventCreated.UpdateDetails);
  }

  [Fact]
  public void RecordsNoEventIfNameDoesNotChange()
  {
    var member = GetMemberWithDefaultName();
    member.UpdateName(_initialFirstName, _initialLastName);

    Assert.Empty(member.Events);
  }

  [Fact]
  public void RecordsEventWithAppendedDetailsIfOtherThingsChanged()
  {
    string newLastName = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultName();
    member.UpdateDiscord("new discord");
    member.UpdateName(_initialFirstName, newLastName);
    var eventCreated = (MemberUpdatedEvent)member.Events.First();

    Assert.Same(member, eventCreated.Member);
    Assert.Equal("Discord Username,Name", eventCreated.UpdateDetails);
  }
}
