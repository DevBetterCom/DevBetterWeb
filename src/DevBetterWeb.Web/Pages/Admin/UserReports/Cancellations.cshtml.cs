using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.UserReports
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

  public class CancellationsModel : PageModel
  {

    private static readonly DateTimeRange defaultRange = new DateTimeRange(DateTime.Today.AddDays(-29), DateTime.Today.AddDays(-1));

    public List<BillingActivity> CancelledBillingActivities { get; set; } = new List<BillingActivity>();

    private readonly IRepository<BillingActivity> _repository;
    private readonly ICsvService _csvService;

    [BindProperty]
    public ReportsDatesModel _cancellationReportsDatesModel { get; set; }

    public CancellationsModel(IRepository<BillingActivity> repository,
      ICsvService csvService)
    {
      _repository = repository;
      _cancellationReportsDatesModel = new ReportsDatesModel(defaultRange.StartDate, (DateTime)defaultRange.EndDate!);
      _csvService = csvService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      DateTimeRange dates = new DateTimeRange((DateTime)_cancellationReportsDatesModel.StartDate!, _cancellationReportsDatesModel.EndDate);

      var spec = new BillingActivitiesByDateTimeRangeAndCancelledVerbSpec(dates);
      CancelledBillingActivities = await _repository.ListAsync(spec);

      return Page();
    }

    public async Task OnPostRefresh()
    {
      DateTimeRange dates = new DateTimeRange((DateTime)_cancellationReportsDatesModel.StartDate!, _cancellationReportsDatesModel.EndDate);

      var spec = new BillingActivitiesByDateTimeRangeAndCancelledVerbSpec(dates);
      CancelledBillingActivities = await _repository.ListAsync(spec);
    }

    public async Task<IActionResult> OnPostDownload()
    {
      byte[] array = new byte[] { 0 };

      DateTimeRange dates = new DateTimeRange((DateTime)_cancellationReportsDatesModel.StartDate!, _cancellationReportsDatesModel.EndDate);

      var spec = new BillingActivitiesByDateTimeRangeAndCancelledVerbSpec(dates);
      CancelledBillingActivities = await _repository.ListAsync(spec);

      if (CancelledBillingActivities.Count != 0)
      {
        array = _csvService.GetCsvByteArrayFromList(CancelledBillingActivities);
      }

      return new FileContentResult(array, "text/csv")
      {
        FileDownloadName = "CancellationsList-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv"
      };
    }
  }
}

