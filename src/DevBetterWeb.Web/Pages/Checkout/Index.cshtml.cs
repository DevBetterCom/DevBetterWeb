using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.Pages.Checkout;

public class IndexModel : PageModel
{
  //public readonly IOptions<StripeOptions> options;
  //public string? StripePublishableKey { get; private set; }

  //public IndexModel(IOptions<StripeOptions> _options)
  //{
  //  options = _options;
  //  StripePublishableKey = options.Value.stripePublishableKey;
  //}

  public void OnGet()
  {
  }

}
