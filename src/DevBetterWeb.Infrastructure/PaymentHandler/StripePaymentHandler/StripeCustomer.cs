using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripeCustomer : IPaymentHandlerCustomer
  {
    public string GetCustomerEmail(string customerId)
    {
      var customer = GetCustomer(customerId);

      var email = customer.Email;

      return email;

    }

    private Stripe.Customer GetCustomer(string customerId)
    {
      var customerService = new CustomerService();

      var customer = customerService.Get(customerId);

      return customer;
    }
  }

}
