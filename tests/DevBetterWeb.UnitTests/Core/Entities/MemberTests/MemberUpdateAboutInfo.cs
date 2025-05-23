﻿using System;
using System.Linq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberTests
{
  private string _initialAboutInfo = "";

  private Member GetMemberWithDefaultAboutInfo()
  {
    _initialAboutInfo = Guid.NewGuid().ToString();

    Member? member = MemberHelpers.CreateWithDefaultConstructor();
    member.UpdateAboutInfo(_initialAboutInfo);
    member.ClearDomainEvents();

    return member;
  }

  [Fact]
  public void SetsAboutInfo()
  {
    string newAboutInfo = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultAboutInfo();
    member.UpdateAboutInfo(newAboutInfo);

    Assert.Equal(newAboutInfo, member.AboutInfo);
  }

  [Fact]
  public void RecordsEventIfAboutInfoChanges()
  {
    string newAboutInfo = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultAboutInfo();
    member.UpdateAboutInfo(newAboutInfo);
    var eventCreated = (MemberUpdatedEvent)member.DomainEvents.First();

    Assert.Same(member, eventCreated.Member);
    Assert.Equal("AboutInfo", eventCreated.UpdateDetails);
  }

  [Fact]
  public void RecordsNoEventIfAboutInfoDoesNotChange()
  {
    var member = GetMemberWithDefaultAboutInfo();
    member.UpdateAboutInfo(_initialAboutInfo);

    Assert.Empty(member.DomainEvents);
  }

  [Fact]
  public void RecordsEventWithAppendedDetailsIfOtherThingsChanged()
  {
    string newAboutInfo = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultAboutInfo();
    member.UpdateName("kylo", "ren");
    member.UpdateAboutInfo(newAboutInfo);
    var eventCreated = (MemberUpdatedEvent)member.DomainEvents.First();

    Assert.Same(member, eventCreated.Member);
    Assert.Equal("Name,AboutInfo", eventCreated.UpdateDetails);
  }
}
