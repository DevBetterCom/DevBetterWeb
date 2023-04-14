using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects;

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

  /// <summary>
  /// Returns total number of days in the DateTimeRange.
  /// If no EndDate has been specified, will use provided endDateToUseIfMissing instead.
  /// </summary>
  /// <param name="endDateToUseIfMissing"></param>
  /// <returns></returns>
  public int ToDays(DateTime endDateToUseIfMissing)
  {
    var end = EndDate ?? endDateToUseIfMissing;

    return (end - StartDate).Days;
  }

  /// <summary>
  /// Returns number of days the DateTimeRange covers from its start until some period end date.
  /// If the DateTimeRange EndDate is less than the periodEndDate, returns ToDays()
  /// </summary>
  /// <param name="periodEndDate"></param>
  /// <returns></returns>
  public int ToDaysInRangeThroughPeriodEndDate(DateTime periodEndDate)
  {
    DateTime end = periodEndDate;
    if (EndDate.HasValue && EndDate.Value <= periodEndDate)
    {
      return ToDays(EndDate.Value);
    }
    return (end - StartDate).Days;
  }

  public bool Contains(DateTime date)
  {
    if (date >= StartDate && date <= EndDate)
    {
      return true;
    }
    return false;
  }

  protected override IEnumerable<IComparable> GetEqualityComponents()
  {
    yield return StartDate;
    yield return EndDate ?? DateTime.MaxValue;
  }
}
