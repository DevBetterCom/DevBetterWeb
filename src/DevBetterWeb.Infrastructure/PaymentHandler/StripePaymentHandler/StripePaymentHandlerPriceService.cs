using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerPriceService : IPaymentHandlerPrice
  {
    private readonly PriceService _priceService;

    public StripePaymentHandlerPriceService(PriceService priceService)
    {
      _priceService = priceService;
    }

    public int GetPriceAmount(string subscriptionPriceId)
    {
      var priceObject = _priceService.Get(subscriptionPriceId);

      int price = (int)priceObject.UnitAmount!;

      return price;
    }
  }

}
