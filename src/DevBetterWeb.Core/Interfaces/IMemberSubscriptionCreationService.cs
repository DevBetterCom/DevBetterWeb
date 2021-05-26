using System.Threading.Tasks;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberSubscriptionCreationService
  {
    Task CreateSubscriptionForMemberAsync(int memberId, DateTimeRange subscriptionDateTimeRange);
  }
}
