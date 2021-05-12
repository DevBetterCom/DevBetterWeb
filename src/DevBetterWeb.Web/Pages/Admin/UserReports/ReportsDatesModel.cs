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

    public ReportsDatesModel(DateTime startDate, DateTime inclusiveEndDate)
    {
      StartDate = startDate;
      EndDate = new DateTime(inclusiveEndDate.Year, inclusiveEndDate.Month, inclusiveEndDate.Day, 23, 59, 59);
    }
  }
}
