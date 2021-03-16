using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerPaymentIntentService : IPaymentHandlerPaymentIntent
  {
    public string CreatePaymentIntent(int orderAmount)
    {
      var paymentIntents = new PaymentIntentService();
      var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
      {
        Amount = orderAmount,
        Currency = "usd",
        SetupFutureUsage = "off_session",
      });

      var paymentIntentId = paymentIntent.Id;

      return paymentIntentId;
    }

    public string GetClientSecret(string paymentIntentId)
    {
      throw new System.NotImplementedException();
    }

    public void UpdatePaymentIntent(string paymentIntentSecret, string customerId)
    {
      var paymentIntents = new PaymentIntentService();

      paymentIntents.Update(
        paymentIntentSecret,
        new PaymentIntentUpdateOptions
        {
          Customer = customerId,
        });
    }
  }

}
