using System;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities
{
  public class BillingActivity : BaseEntity
  {
    public int MemberId { get; private set; }
    public BillingDetails Details { get; private set; }

    public BillingActivity(int memberId, BillingDetails details)
    {
      MemberId = memberId;
      Details = details;
    }

    private BillingActivity()
    {
      MemberId = 0;
      Details = new BillingDetails();
    }
  }
}
