using Ardalis.GuardClauses;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.Pages.Checkout
{
  public class MonthlyModel : PageModel
  {

    public SubscriptionTypeViewModel SubscriptionType { get; set; }

    public MonthlyModel(IOptions<StripeOptions> optionsAccessor)
    {
      Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
      Guard.Against.NullOrEmpty(optionsAccessor.Value.stripePublishableKey, nameof(optionsAccessor.Value.stripePublishableKey));
      SubscriptionType = new SubscriptionTypeViewModel("Monthly", "month", 200,
        optionsAccessor.Value.stripePublishableKey, optionsAccessor.Value.monthlyPlanId!);
    }

    public void OnGet()
    {
    }

  }
}
