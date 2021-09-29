using System;
using DevBetterWeb.Core.ValueObjects;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.ValueObjects
{
  public class DateTimeRangeToDaysInRangeThroughPeriodEndDate
  {
    [Fact]
    public void Returns10Given10DayRangeAndPeriodEndDateBeyondEndDate()
    {
      var range = new DateTimeRange(DateTime.Today.AddDays(-10), DateTime.Today);

      Assert.Equal(10, range.ToDaysInRangeThroughPeriodEndDate(DateTime.Today.AddDays(5)));
    }

    [Fact]
    public void Returns10Given10DayRangeWithoutEndDate()
    {
      var range = new DateTimeRange(DateTime.Today.AddDays(-10), null);

      Assert.Equal(10, range.ToDays(DateTime.Today));
    }

    [Fact]
    public void Returns5Given10DayRangeWithPeriodEndDate5DaysFromStartDate()
    {
      var range = new DateTimeRange(DateTime.Today.AddDays(-10), DateTime.Today);

      Assert.Equal(10, range.ToDays(DateTime.Today.AddDays(-5)));
    }
  }
}
