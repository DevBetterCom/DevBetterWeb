namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerSubscriptionCreationService
  {
    IPaymentHandlerSubscriptionDTO SetUpSubscription(string customerId, string priceId, string paymentMethodId);
  }
}
