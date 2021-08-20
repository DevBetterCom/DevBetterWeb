namespace DevBetterWeb.Infrastructure.PaymentHandler
{
  public class PaymentHandlerCustomer
  {
    public string CustomerId { get; set; }
    public string Email { get; set; }

    public PaymentHandlerCustomer(string customerId, string email)
    {
      CustomerId = customerId;
      Email = email;
    }
  }
}
