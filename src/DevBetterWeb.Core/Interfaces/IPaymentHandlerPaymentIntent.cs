namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerPaymentIntent
  {
    string CreatePaymentIntent(int orderAmount);
    void UpdatePaymentIntent(string paymentIntentSecret, string customerId);
    string GetClientSecret(string paymentIntentId);
  }
}
