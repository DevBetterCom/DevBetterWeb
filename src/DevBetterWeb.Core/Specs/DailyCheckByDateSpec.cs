using Ardalis.Specification;
using DevBetterWeb.Core.Entities;
using System;

namespace DevBetterWeb.Core.Specs
{
  public class DailyCheckByDateSpec : Specification<DailyCheck>, ISingleResultSpecification
  {
    public DailyCheckByDateSpec(DateTime date)
    {
      Query.Where(dc => dc.Date.Date.Equals(date.Date))
        .Take(1);
    }
  }
}
