using System.Threading.Tasks;
using DevBetterWeb.Core.Enums;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberAddBillingActivityService
  {
    Task AddMemberSubscriptionCreationBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod);
    Task AddMemberSubscriptionRenewalBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod);
    Task AddMemberSubscriptionCancellationBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod);
    Task AddMemberSubscriptionEndingBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod);
  }
}
