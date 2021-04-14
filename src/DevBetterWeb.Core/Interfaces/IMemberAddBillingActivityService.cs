using System.Threading.Tasks;
using DevBetterWeb.Core.Enums;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberAddBillingActivityService
  {
    Task AddSubscriptionCreationBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod);
    Task AddSubscriptionRenewalBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod);
    Task AddSubscriptionCancellationBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod);
    Task AddSubscriptionEndingBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod);
  }
}
