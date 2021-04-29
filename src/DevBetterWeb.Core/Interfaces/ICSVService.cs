using System.Collections.Generic;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces
{
  public interface ICSVService
  {
    byte[] GetCSVByteArrayFromList(List<BillingActivity> billingActivities);
  }
}
