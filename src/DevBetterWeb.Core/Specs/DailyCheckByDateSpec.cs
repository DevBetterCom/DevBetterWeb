using System;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class DailyCheckByDateSpec : Specification<DailyCheck>, 
	ISingleResultSpecification<DailyCheck>
{
  public DailyCheckByDateSpec(DateTime date)
  {
    Query.Where(dc => dc.Date.Date.Equals(date.Date))
      .Take(1);
  }
}
