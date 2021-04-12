using System;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberSubscriptionRenewalService
  {
    Task ExtendMemberSubscription(string email, DateTime subscriptionEndDate);
  }
}
