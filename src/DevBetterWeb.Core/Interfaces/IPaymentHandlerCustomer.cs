namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerCustomer
  {
    string GetCustomerEmail(string customerId);
    string CreateCustomer(string email);
  }
}
