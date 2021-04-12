using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberAddBillingActivityService
  {
    Task AddSubscriptionCreationBillingActivity(string email, decimal amount);
    Task AddSubscriptionRenewalBillingActivity(string email, decimal amount);
    Task AddSubscriptionCancellationBillingActivity(string email);
    Task AddSubscriptionEndingBillingActivity(string email);
  }
}
