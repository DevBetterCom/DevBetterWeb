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

    public string CreateCustomer(string email)
    {
      var customer = _customerService.Create(new CustomerCreateOptions
      {
        Email = email,
      });

      return customer.Id;
    }

    public string GetCustomerEmail(string customerId)
    {
      var customer = GetCustomer(customerId);

      var email = customer.Email;

      return email;

    }

    public void UpdatePaymentMethod(string customerId, string paymentMethodId)
    {
      var customerOptions = new CustomerUpdateOptions
      {
        InvoiceSettings = new CustomerInvoiceSettingsOptions
        {
          DefaultPaymentMethod = paymentMethodId,
        },
      };
      _customerService.Update(customerId, customerOptions);
    }

    private Stripe.Customer GetCustomer(string customerId)
    {
      var customer = _customerService.Get(customerId);

      return customer;
    }
  }

}
