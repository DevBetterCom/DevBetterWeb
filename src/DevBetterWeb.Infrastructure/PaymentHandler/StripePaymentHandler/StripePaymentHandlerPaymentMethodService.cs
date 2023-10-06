using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;

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

  public string PaymentMethodCreateByCard(string customerEmail, string cardNumber, int cardExpMonth,
	  int cardExpYear, string cardCvc)
  {
		var options = new PaymentMethodCreateOptions
		{
		  Type = "card",
		  Card = new PaymentMethodCardOptions
		  {
			  Number = cardNumber,
			  ExpMonth = cardExpMonth,
			  ExpYear = cardExpYear,
			  Cvc = cardCvc,
		  },
		  BillingDetails = new PaymentMethodBillingDetailsOptions
		  {
			  Email = customerEmail
			}
		};

		var paymentMethod = _paymentMethodService.Create(options);

		return paymentMethod.Id;
  }
}
