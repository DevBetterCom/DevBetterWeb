using System.Threading.Tasks;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberSubscriptionFactory
  {
    Task CreateSubscriptionForMemberAsync(int memberId, DateTimeRange subscriptionDateTimeRange);
  }
}
