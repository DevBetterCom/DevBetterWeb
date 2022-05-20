using System;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.UnitTests.Core.Entities;
using FluentAssertions;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Events;

public class SubscriptionUpdatedEventConstructor
{
  private readonly Member _member = MemberHelpers.CreateWithDefaultConstructor();
  private readonly MemberSubscription _memberSubscription = SubscriptionHelpers.GetDefaultTestSubscription();

  [Fact]
  public void ShouldSetValues()
  {
    var sut = new SubscriptionUpdatedEvent(_member, _memberSubscription);

    sut.Member.Should().Be(_member);
    sut.MemberSubscription.Should().Be(_memberSubscription);
  }

  [Fact]
  public void ShouldThrowExceptionWhenMemberIsNull()
  {
    var action = () => new SubscriptionUpdatedEvent(null, _memberSubscription);

    action.Should().Throw<ArgumentNullException>();
  }
    
  [Fact]
  public void ShouldThrowExceptionWhenMemberSubscriptionIsNull()
  {
    var action = () => new SubscriptionUpdatedEvent(_member, null);

    action.Should().Throw<ArgumentNullException>();
  }
}
