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

}
