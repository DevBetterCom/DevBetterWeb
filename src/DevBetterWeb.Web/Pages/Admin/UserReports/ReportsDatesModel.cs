using System;

namespace DevBetterWeb.Web.Pages.Admin.UserReports
{
  public class ReportsDatesModel
  {
    public DateTime? StartDate { get; set; } 
    public DateTime? EndDate { get; set; } 

    public ReportsDatesModel()
    {
      // for model binding
    }

    public ReportsDatesModel(DateTime startDate, DateTime endDate)
    {
      StartDate = startDate;
      EndDate = endDate;
    }
  }
}
