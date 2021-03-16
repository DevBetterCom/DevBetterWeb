namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerPaymentIntent
  {
    string CreatePaymentIntent(int orderAmound);
    string GetClientSecret(string paymentIntentId);
  }
}
