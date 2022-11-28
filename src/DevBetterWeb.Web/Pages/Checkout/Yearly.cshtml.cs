using Ardalis.GuardClauses;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.Pages.Checkout;

public class YearlyModel : PageModel
{

  public SubscriptionTypeViewModel SubscriptionType { get; set; }


  public YearlyModel(IOptions<StripeOptions> optionsAccessor)
  {
    Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
    Guard.Against.NullOrEmpty(optionsAccessor.Value.StripePublishableKey, nameof(optionsAccessor.Value.StripePublishableKey));
    SubscriptionType = new SubscriptionTypeViewModel("Yearly", "year", 2000,
      optionsAccessor.Value.StripePublishableKey, optionsAccessor.Value.YearlyPlanId!);
  }

  public void OnGet()
  {
  }
}
