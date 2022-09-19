using Ardalis.GuardClauses;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.Pages.Checkout;

public class MonthlyModel : PageModel
{

  public SubscriptionTypeViewModel SubscriptionType { get; set; }

  public MonthlyModel(IOptions<StripeOptions> optionsAccessor)
  {
    Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
    Guard.Against.NullOrEmpty(optionsAccessor.Value.StripePublishableKey, nameof(optionsAccessor.Value.StripePublishableKey));
    SubscriptionType = new SubscriptionTypeViewModel("Monthly", "month", 200,
      optionsAccessor.Value.StripePublishableKey, optionsAccessor.Value.MonthlyPlanId!);
  }

  public void OnGet()
  {
  }

}
