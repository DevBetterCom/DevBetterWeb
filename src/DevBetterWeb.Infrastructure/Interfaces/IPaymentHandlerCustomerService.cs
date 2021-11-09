using DevBetterWeb.Infrastructure.PaymentHandler;

namespace DevBetterWeb.Infrastructure.Interfaces;

public interface IPaymentHandlerCustomerService
{
  PaymentHandlerCustomer GetCustomer(string customerId);
  PaymentHandlerCustomer CreateCustomer(string email);
  void SetPaymentMethodAsCustomerDefault(string customerId, string paymentMethodId);
}
