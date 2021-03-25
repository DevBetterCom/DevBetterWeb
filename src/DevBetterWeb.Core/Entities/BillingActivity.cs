using System;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities
{
  public class BillingActivity : BaseEntity
  {
    public int MemberId { get; private set; }
    public DateTime Date { get; private set; }
    public BillingDetails Details { get; private set; }

    public BillingActivity(int memberId, DateTime date, BillingDetails details)
    {
      MemberId = memberId;
      Date = date;
      Details = details;
    }

    private BillingActivity()
    {
      MemberId = 0;
      Date = DateTime.MinValue;
      Details = new BillingDetails("");
    }
  }
}
