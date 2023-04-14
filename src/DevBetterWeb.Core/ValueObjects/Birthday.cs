using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects;

public class Birthday : ValueObject
{
  public int Day { get; }
  public int Month { get; }
  private const int LEAP_YEAR = 2020;

  public Birthday(int day, int month)
  {
    _ = new DateOnly(LEAP_YEAR, month, day); // will throw if invalid day/month combination
    Day = day;
    Month = month;
  }

  protected override IEnumerable<IComparable> GetEqualityComponents()
  {
    yield return Day;
    yield return Month;
  }

  public override string ToString()
  {
    return new DateOnly(LEAP_YEAR, Month, Day).ToString("MMMM d");
  }
}
