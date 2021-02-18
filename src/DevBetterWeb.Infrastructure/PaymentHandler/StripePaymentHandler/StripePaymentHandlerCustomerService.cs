using DevBetterWeb.Core.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerCustomerService : IPaymentHandlerCustomer
  {
    private readonly CustomerService _customerService;

    public StripePaymentHandlerCustomerService(CustomerService customerService)
    {
      _customerService = customerService;
    }

    public string GetCustomerEmail(string customerId)
    {
      var customer = GetCustomer(customerId);

      var email = customer.Email;

      return email;

    }

    private Stripe.Customer GetCustomer(string customerId)
    {
      var customer = _customerService.Get(customerId);

      return customer;
    }
  }

}
