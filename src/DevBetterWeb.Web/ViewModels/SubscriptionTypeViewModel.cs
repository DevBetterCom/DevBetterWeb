namespace DevBetterWeb.Web.ViewModels;

public class SubscriptionTypeViewModel
{
  public string Name { get; set; }
  public string SubscriptionPeriod { get; set; }
  public int SubscriptionPrice { get; set; }
  public string StripePublishableKey { get; set; }
  public string SubscriptionPlanPriceId { get; set; }

  public SubscriptionTypeViewModel(string name, string subscriptionPeriod, int subscriptionPrice,
    string stripePublishableKey, string subscriptionPlanPriceId)
  {
    Name = name;
    SubscriptionPeriod = subscriptionPeriod;
    SubscriptionPrice = subscriptionPrice;

    StripePublishableKey = stripePublishableKey;
    SubscriptionPlanPriceId = subscriptionPlanPriceId;
  }

}
