using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Specs
{
  public class BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec : Specification<BillingActivity>
  {
    public BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec(DateTimeRange range)
    {
      Query.Where(activity => range.Contains(activity.Details.Date))
        .OrderBy(activity => activity.Details.Date);
    }
  }
}
