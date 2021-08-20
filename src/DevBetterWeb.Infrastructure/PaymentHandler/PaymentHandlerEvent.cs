namespace DevBetterWeb.Infrastructure.PaymentHandler
{
  public class PaymentHandlerEvent
  {
    public string EventType {  get; set; }
    public string InvoiceId { get; set; }
    public string SubscriptionId {  get; set; }

    public PaymentHandlerEvent(string eventType, string invoiceId, string subscriptionId)
    {
      EventType = eventType;
      InvoiceId = invoiceId;
      SubscriptionId = subscriptionId;
    }
  }
}
