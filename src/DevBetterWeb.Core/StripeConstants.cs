namespace DevBetterWeb.Core
{
  public static class StripeConstants
  {
    // public const string CUSTOMER_SUBSCRIPTION_CREATED_EVENT_TYPE = "customer.subscription.created";
    public const string CUSTOMER_SUBSCRIPTION_UPDATED_EVENT_TYPE = "customer.subscription.updated";
    public const string CUSTOMER_SUBSCRIPTION_DELETED_EVENT_TYPE = "customer.subscription.deleted";

    public const string INVOICE_PAYMENT_SUCCEEDED_EVENT_TYPE = "invoice.payment_succeeded";
    public const string INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_RENEWAL = "subscription_cycle";
    public const string INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_CREATION = "subscription_create";
  }
}
