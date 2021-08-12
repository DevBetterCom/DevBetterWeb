namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerEventService
  {
    string GetEventType(string json);
    string GetSubscriptionId(string json);
    string GetInvoiceId(string json);
  }
}
