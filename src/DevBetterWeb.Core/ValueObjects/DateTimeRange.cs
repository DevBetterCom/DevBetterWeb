using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects
{

  public class DateTimeRange : ValueObject
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

    public int ToDaysToDate(DateTime endDate)
    {
      var end = endDate;
      return (end - StartDate).Days;
    }

    public bool Contains(DateTime date)
    {
      if(date >= StartDate && date <= EndDate)
      {
        return true;
      }
      return false;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return StartDate;
      yield return EndDate ?? DateTime.MaxValue;
    }
  }
}
