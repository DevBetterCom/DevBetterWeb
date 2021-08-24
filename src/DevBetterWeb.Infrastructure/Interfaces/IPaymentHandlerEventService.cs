using DevBetterWeb.Infrastructure.PaymentHandler;

namespace DevBetterWeb.Infrastructure.Interfaces
{
  public interface IPaymentHandlerEventService
  {
    //string GetEventType(string json);
    //string GetSubscriptionId(string json);
    //string GetInvoiceId(string json);
    PaymentHandlerEvent FromJson(string json);
  }
}
