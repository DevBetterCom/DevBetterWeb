using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;

public class StripePaymentHandlerSubscriptionDTO : IPaymentHandlerSubscriptionDTO
{
  public string? _id { get; private set; }
  public string? _status { get; private set; }
  public string? _latestInvoicePaymentIntentStatus { get; private set; }
  public string? _latestInvoicePaymentIntentClientSecret { get; private set; }
  public string? _errorMessage { get; private set; }


  public StripePaymentHandlerSubscriptionDTO(string id, string status, string latestInvoicePaymentIntentStatus, string latestInvoicePaymentIntentClientSecret)
  {
    _id = id;
    _status = status;
    _latestInvoicePaymentIntentStatus = latestInvoicePaymentIntentStatus;
    _latestInvoicePaymentIntentClientSecret = latestInvoicePaymentIntentClientSecret;
  }

  public StripePaymentHandlerSubscriptionDTO(string errorMessage)
  {
    _errorMessage = errorMessage;
  }

}
