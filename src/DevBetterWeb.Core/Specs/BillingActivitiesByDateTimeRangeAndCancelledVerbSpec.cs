using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Specs
{
  public class BillingActivitiesByDateTimeRangeAndCancelledVerbSpec : Specification<BillingActivity>
  {
    public BillingActivitiesByDateTimeRangeAndCancelledVerbSpec(DateTimeRange range)
    {
      Query.Where(activity => activity.Details.ActionVerbPastTense == Enums.BillingActivityVerb.Cancelled)
        .Where(activity => activity.Details.Date >= range.StartDate)
        .Where(activity => activity.Details.Date <= range.EndDate)
        .OrderBy(activity => activity.Details.Date);
    }
  }
}
