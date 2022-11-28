namespace DevBetterWeb.Infrastructure.Services;

public class StripeOptions
{
  public string? StripePublishableKey { get; set; }
  public string? StripeSecretKey { get; set; }
  public string? YearlyPlanId { get; set; }
  public string? MonthlyPlanId { get; set; }
  public string? StripeInvoicePaidWebHookSecretKey { get; set; }
  public string? StripePaymentIntentSucceededWebHookSecretKey { get; set; }
  public string? StripeCustomerSubscriptionUpdatedWebHookSecretKey { get; set; }
  public string? StripeCustomerSubscriptionDeletedWebHookSecretKey { get; set; }
}
