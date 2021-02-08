namespace DevBetterWeb.Infrastructure.Services
{
  public class StripeOptions
  {
    public string? stripePublishableKey { get; set; }
    public string? stripeSecretKey { get; set; }
    public string? yearlyPlanId { get; set; }
    public string? monthlyPlanId { get; set; }

  }
}
