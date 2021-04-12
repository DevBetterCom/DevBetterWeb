using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IWebhookHandlerService
  {
    Task HandleNewCustomerSubscriptionAsync(string json);
    Task HandleCustomerSubscriptionRenewedAsync(string json);
    Task HandleCustomerSubscriptionEndedAsync(string json);
    Task HandleCustomerSubscriptionCancelledAtPeriodEndAsync(string json);
  }
}
