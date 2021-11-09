using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;

public class StripePaymentHandlerPaymentIntentService : IPaymentHandlerPaymentIntent
{
  private readonly PaymentIntentService _paymentIntentService;

  public StripePaymentHandlerPaymentIntentService(PaymentIntentService paymentIntentService)
  {
    _paymentIntentService = paymentIntentService;
  }

  public string CreatePaymentIntent(int orderAmount)
  {
    var paymentIntent = _paymentIntentService.Create(new PaymentIntentCreateOptions
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
    var paymentIntent = _paymentIntentService.Get(paymentIntentId);
    var clientSecret = paymentIntent.ClientSecret;

    return clientSecret;
  }

  public void UpdatePaymentIntent(string paymentIntentSecret, string customerId)
  {
    _paymentIntentService.Update(
      paymentIntentSecret,
      new PaymentIntentUpdateOptions
      {
        Customer = customerId,
      });
  }
}
