namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerEvent
  {
    string GetEventType(string json);
    string GetSubscriptionId(string json);
    string GetInvoiceId(string json);
  }
}
