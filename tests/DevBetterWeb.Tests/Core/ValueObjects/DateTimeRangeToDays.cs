using System;
using DevBetterWeb.Core.ValueObjects;
using Xunit;

namespace DevBetterWeb.Tests.Core.ValueObjects
{
  public class DateTimeRangeToDays
  {
    [Fact]
    public void Returns10Given10DayRange()
    {
      var range = new DateTimeRange(DateTime.Today.AddDays(-10), DateTime.Today);

      Assert.Equal(10, range.ToDays(DateTime.Today));
    }

    [Fact]
    public void Returns10Given10DayRangeWithoutEndDate()
    {
      var range = new DateTimeRange(DateTime.Today.AddDays(-10), null);

      Assert.Equal(10, range.ToDays(DateTime.Today));
    }
  }
}
