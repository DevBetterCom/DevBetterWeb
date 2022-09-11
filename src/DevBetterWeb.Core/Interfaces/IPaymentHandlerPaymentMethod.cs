namespace DevBetterWeb.Core.Interfaces;

public interface IPaymentHandlerPaymentMethod
{
  void AttachPaymentMethodToCustomer(string paymentMethodId, string customerId);
  string PaymentMethodCreateByCard(string customerEmail, string cardNumber, int cardExpMonth, int cardExpYear,
	  string cardCvc);
}
