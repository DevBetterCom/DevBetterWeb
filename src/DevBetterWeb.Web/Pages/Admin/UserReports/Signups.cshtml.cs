using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Authorization;
using DevBetterWeb.Core;

namespace DevBetterWeb.Web.Pages.Admin.UserReports
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class SignupsModel : PageModel
  {
    private static readonly DateTimeRange defaultRange = new DateTimeRange(DateTime.Today.AddDays(-29), DateTime.Today.AddDays(-1));

    public List<BillingActivity> SubscribedBillingActivities { get; set; } = new List<BillingActivity>();

    private readonly IRepository<BillingActivity> _repository;

    [BindProperty]
    public SignupReportsDatesModel _signupReportsDatesModel { get; set; }

    public SignupsModel(IRepository<BillingActivity> repository)
    {
      _repository = repository;
      _signupReportsDatesModel = new SignupReportsDatesModel(defaultRange.StartDate, (DateTime)defaultRange.EndDate!);
    }

    public async Task<IActionResult> OnGetAsync()
    {
      DateTimeRange dates = new DateTimeRange((DateTime)_signupReportsDatesModel.StartDate!, _signupReportsDatesModel.EndDate);

      var spec = new BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec(dates);
      SubscribedBillingActivities = await _repository.ListAsync(spec);

      return Page();
    }

    public async Task OnPostAsync()
    {
      DateTimeRange dates = new DateTimeRange((DateTime)_signupReportsDatesModel.StartDate!, _signupReportsDatesModel.EndDate);

      var spec = new BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec(dates);
      SubscribedBillingActivities = await _repository.ListAsync(spec);
    }
  }

  public class SignupReportsDatesModel
  {
    public DateTime? StartDate { get; set; } 
    public DateTime? EndDate { get; set; } 

    public SignupReportsDatesModel()
    {
      // for model binding
    }

    public SignupReportsDatesModel(DateTime startDate, DateTime endDate)
    {
      StartDate = startDate;
      EndDate = endDate;
    }
  }
}
