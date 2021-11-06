using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Specs;

public class BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec : Specification<BillingActivity>
{
  public BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec(DateTimeRange range)
  {
    Query.Where(activity => activity.Details.ActionVerbPastTense == Enums.BillingActivityVerb.Subscribed)
      .Where(activity => activity.Details.Date >= range.StartDate)
      .Where(activity => activity.Details.Date <= range.EndDate)
      .OrderBy(activity => activity.Details.Date);
  }
}
