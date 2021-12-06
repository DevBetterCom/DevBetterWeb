using System.Collections.Generic;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface ICsvService
{
  byte[] GetCsvByteArrayFromList(List<BillingActivity> billingActivities);
}
