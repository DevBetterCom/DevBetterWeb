using System;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.UnitTests.Core.Entities;
using FluentAssertions;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Events;

public class SubscriptionAddedEventConstructor
{
  private readonly Member _member = MemberHelpers.CreateWithDefaultConstructor();
  private readonly MemberSubscription _memberSubscription = SubscriptionHelpers.GetDefaultTestSubscription();

  [Fact]
  public void ShouldSetValues()
  {
    var sut = new SubscriptionAddedEvent(_member, _memberSubscription);

    sut.Member.Should().Be(_member);
    sut.MemberSubscription.Should().Be(_memberSubscription);
  }

  [Fact]
  public void ShouldThrowExceptionWhenMemberIsNull()
  {
    var action = () => new SubscriptionAddedEvent(null, _memberSubscription);

    action.Should().Throw<ArgumentNullException>();
  }
    
  [Fact]
  public void ShouldThrowExceptionWhenMemberSubscriptionIsNull()
  {
    var action = () => new SubscriptionAddedEvent(_member, null);

    action.Should().Throw<ArgumentNullException>();
  }
}
