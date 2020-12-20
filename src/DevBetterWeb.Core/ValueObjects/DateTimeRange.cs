using System;

namespace DevBetterWeb.Core.ValueObjects
{
  public class DateTimeRange //: ValueObject
  {
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public DateTimeRange(DateTime startDate, DateTime? endDate)
    {
      if (endDate != null && startDate > endDate) throw new ArgumentException("The end date cannot be before the start date");

      StartDate = startDate;
      EndDate = endDate;
    }

    public int ToDays(DateTime endDateToUseIfMissing)
    {
      var end = EndDate ?? endDateToUseIfMissing;

      return (end - StartDate).Days;
    }
  }
}
