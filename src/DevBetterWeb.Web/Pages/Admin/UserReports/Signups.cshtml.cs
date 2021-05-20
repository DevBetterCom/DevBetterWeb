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
    private readonly ICsvService _csvService;

    [BindProperty]
    public ReportsDatesModel _signupReportsDatesModel { get; set; }

    public SignupsModel(IRepository<BillingActivity> repository,
      ICsvService csvService)
    {
      _repository = repository;
      _signupReportsDatesModel = new ReportsDatesModel(defaultRange.StartDate, (DateTime)defaultRange.EndDate!);
      _csvService = csvService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      DateTimeRange dates = new DateTimeRange((DateTime)_signupReportsDatesModel.StartDate!, _signupReportsDatesModel.EndDate);

      var spec = new BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec(dates);
      SubscribedBillingActivities = await _repository.ListAsync(spec);

      return Page();
    }

    public async Task OnPostRefresh()
    {
      DateTimeRange dates = new DateTimeRange((DateTime)_signupReportsDatesModel.StartDate!, _signupReportsDatesModel.EndDate);

      var spec = new BillingActivitiesByDateTimeRangeAndSubscribedVerbSpec(dates);
      SubscribedBillingActivities = await _repository.ListAsync(spec);
    }

    public IActionResult OnPostDownload()
    {
      byte[] array = new byte[] { 0 };

      if(SubscribedBillingActivities.Count != 0)
      {
        array = _csvService.GetCsvByteArrayFromList(SubscribedBillingActivities);
      }

      return new FileContentResult(array, "text/csv")
      {
        FileDownloadName = "SignupsList-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv"
      };
    }
  }
}
