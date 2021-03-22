using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerPaymentMethodService : IPaymentHandlerPaymentMethod
  {
    private readonly PaymentMethodService _paymentMethodService;

    public StripePaymentHandlerPaymentMethodService(PaymentMethodService paymentMethodService)
    {
      _paymentMethodService = paymentMethodService;
    }

    public void AttachPaymentMethodToCustomer(string paymentMethodId, string customerId)
    {
      var options = new PaymentMethodAttachOptions
      {
        Customer = customerId,
      };
      _paymentMethodService.Attach(paymentMethodId, options);
    }
  }

}
