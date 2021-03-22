namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerEvent
  {
    string GetEventType(string json);
    string GetSubscriptionId(string json);
  }
}
