namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerPrice
  {
    int GetPriceAmount(string subscriptionPriceId);
  }

  public interface IPaymentHandlerPaymentIntent
  {
    string CreatePaymentIntent(int orderAmound);
    string GetClientSecret(string paymentIntentId);
  }
}
