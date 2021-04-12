namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerInvoice
  {
    string GetSubscriptionId(string json);
    string GetBillingReason(string json);
    string GetCustomerId(string json);
    decimal GetPaymentAmount(string json);
  }
}
