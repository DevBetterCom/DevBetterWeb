namespace DevBetterWeb.Core.Interfaces;

public interface IPaymentHandlerSubscriptionDTO
{
  public string? _id { get; }
  public string? _status { get; }
  public string? _latestInvoicePaymentIntentStatus { get; }
  public string? _errorMessage { get; }
}
