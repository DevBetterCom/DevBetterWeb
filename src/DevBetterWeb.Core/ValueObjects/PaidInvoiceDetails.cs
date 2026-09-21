namespace DevBetterWeb.Core.ValueObjects;

public record PaidInvoiceDetails(string InvoiceId, string SubscriptionId, string BillingReason, string Status, decimal Total);
