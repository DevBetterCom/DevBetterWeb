namespace DevBetterWeb.Core.Interfaces
{
  // TODO: Make this a single method that returns a DTO with all the things
  public interface IPaymentHandlerEventService
  {
    string GetEventType(string json);
    string GetSubscriptionId(string json);
    string GetInvoiceId(string json);
  }
}
