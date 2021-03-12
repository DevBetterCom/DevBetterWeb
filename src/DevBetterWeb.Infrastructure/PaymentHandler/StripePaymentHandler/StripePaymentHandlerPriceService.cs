using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerPriceService : IPaymentHandlerPrice
  {
    public int GetPriceAmount(string subscriptionPriceId)
    {
      var service = new PriceService();

      var priceObject = service.Get(subscriptionPriceId);

      int price = (int)priceObject.UnitAmount!;

      return price;
    }
  }

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
  }

}
