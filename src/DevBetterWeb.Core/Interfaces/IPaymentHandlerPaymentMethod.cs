namespace DevBetterWeb.Core.Interfaces
{
  public interface IPaymentHandlerPaymentMethod
  {
    void AttachPaymentMethodToCustomer(string paymentMethodId, string customerId);
  }
}
