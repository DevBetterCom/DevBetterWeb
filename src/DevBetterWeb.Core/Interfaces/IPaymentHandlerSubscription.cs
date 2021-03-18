using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerSubscription
  {
    DateTimeRange GetDateTimeRange(string subscriptionId);
    string GetStatus(string subscriptionId);
    string GetCustomerId(string subscriptionId);
    IPaymentHandlerSubscriptionDTO CreateSubscription(string customerId, string priceId);
  }

  public interface IPaymentHandlerSubscriptionDTO
  {
    public string? _id { get; }
    public string? _status { get; }
    public string? _latestInvoicePaymentIntentStatus { get; }
    public string? _errorMessage { get; }
  }
}
