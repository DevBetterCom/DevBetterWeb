using System;
using System.Linq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.ValueObjects;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberUpdateBirthday
{
  private readonly int _initialDay = 1;
  private readonly int _initialMonth = 1;

  private Member GetMemberWithDefaultBirthday()
  { 
    var member = MemberHelpers.CreateWithDefaultConstructor();
    member.UpdateBirthday(_initialDay, _initialMonth);
    member.Events.Clear();

    return member;
  }

  [Fact]
  public void SetsBirthday()
  { 
    var day = 6;
    var month = 6;
    var newBirthday = new Birthday(day, month);
    var member = GetMemberWithDefaultBirthday();
    member.UpdateBirthday(day, month);

    Assert.Equal(newBirthday, member.Birthday);
  }

  [Fact]
  public void RecordsEventIfBirthdayChanges()
  {
    var day = 3;
    var month = 3;

    var member = GetMemberWithDefaultBirthday();
    member.UpdateBirthday(day, month);
    var eventCreated = (MemberUpdatedEvent)member.Events.First();

    Assert.Same(member, eventCreated.Member);
    Assert.Equal("Birthday", eventCreated.UpdateDetails);
  }

  [Fact]
  public void RecordsNoEventIfBirthdayDoesNotChange()
  {
    var member = GetMemberWithDefaultBirthday();
    member.UpdateBirthday(_initialDay, _initialMonth);

    Assert.Empty(member.Events);
  }

  [Fact]
  public void ThrowsExceptionIfInvalidDay()
  {
    var day = 32;
    var month = 1;
    var member = GetMemberWithDefaultBirthday();

    Assert.Throws<ArgumentOutOfRangeException>(() => member.UpdateBirthday(day, month));
  }

  [Fact]
  public void ThrowsExceptionIfInvalidMonth()
  {
    var day = 1;
    var month = 13;
    var member = GetMemberWithDefaultBirthday();

    Assert.Throws<ArgumentOutOfRangeException>(() => member.UpdateBirthday(day, month));
  }

  [Theory]
  [InlineData(11, null)]
  [InlineData(null, 11)]
  [InlineData(null, null)]
  public void SetsBirthdayToNullIfEitherDayOrMonthIsNull(int? day, int? month)
  {
    var member = GetMemberWithDefaultBirthday();
    
    member.UpdateBirthday(day, month);
    Assert.Null(member.Birthday);
  }
}
