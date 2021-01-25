namespace DevBetterWeb.Infrastructure.Services
{
  public class StripeOptions
  {
    public string? stripePublishableKey { get; set; }
    public string? stripeSecretKey { get; set; }
    public string? yearlyPlanId { get; set; }
    public string? monthlyPlanId { get; set; }

    //public string? WebhookSecret { get; set; }
    //public string? BasicPrice { get; set; }
    //public string? ProPrice { get; set; }
    //public string? Domain { get; set; }
  }
}
