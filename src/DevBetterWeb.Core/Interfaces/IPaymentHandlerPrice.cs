namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerPrice
  {
    int GetPriceAmount(string subscriptionPriceId);
  }
}
