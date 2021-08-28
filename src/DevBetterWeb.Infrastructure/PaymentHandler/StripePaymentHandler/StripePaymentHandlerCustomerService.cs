using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerCustomerService : IPaymentHandlerCustomerService
  {
    private readonly CustomerService _customerService;

    public StripePaymentHandlerCustomerService(CustomerService customerService)
    {
      _customerService = customerService;
    }

    public PaymentHandlerCustomer CreateCustomer(string email)
    {
      var paymentHandlerCustomer = GetCustomerByEmail(email);
      if (paymentHandlerCustomer != null)
      {
        return paymentHandlerCustomer;
      }

      var customer = _customerService.Create(new CustomerCreateOptions
      {
        Email = email,
      });

      return new PaymentHandlerCustomer(customer.Id, customer.Email);
    }

    public void SetPaymentMethodAsCustomerDefault(string customerId, string paymentMethodId)
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

    public PaymentHandlerCustomer GetCustomer(string customerId)
    {
      var customer = _customerService.Get(customerId);

      return new PaymentHandlerCustomer(customer.Id, customer.Email);
    }

    public PaymentHandlerCustomer? GetCustomerByEmail(string email)
    {
      var options = new CustomerListOptions
      {
        Email = email,
        Limit = 1,
      };

      StripeList<Customer> customers = _customerService.List(
        options
      );
      if (customers.Data.Count > 0)
      {
        return new PaymentHandlerCustomer(customers.Data[0].Id, customers.Data[0].Email);
      }

      return null;
    }
  }
}
