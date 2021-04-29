using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Core.Services
{
  public class CsvService : ICsvService
  {
    public byte[] GetCsvByteArrayFromList(List<BillingActivity> billingActivities)
    {
      var dataTable = GenerateDataTable(billingActivities);

      var output = CsvByteArrayFromDataTable(dataTable);

      return output;
    }

    private DataTable GenerateDataTable(List<BillingActivity> billingActivities)
    {
      DataTable activities = new DataTable();

      activities.Columns.Add("DATE", typeof(string));
      activities.Columns.Add("NAME", typeof(string));
      activities.Columns.Add("SUBSCRIPTION PLAN", typeof(string));

      foreach(var activity in billingActivities)
      {
        activities.Rows.Add(activity.Details.Date.ToString("yyyy-MM-dd"), activity.Details.MemberName, activity.Details.SubscriptionPlanName);
      }

      return activities;
    }

    private static byte[] CsvByteArrayFromDataTable(DataTable input)
    {
      var stream = new MemoryStream();
      StreamWriter sw = new StreamWriter(stream);

      for (int i = 0; i < input.Columns.Count; i++)
      {
        sw.Write(input.Columns[i]);
        if (i < input.Columns.Count - 1)
        {
          sw.Write(",");
        }
      }
      sw.Write(sw.NewLine);
      foreach (DataRow row in input.Rows)
      {
        for (int i = 0; i < input.Columns.Count; i++)
        {
          if (!Convert.IsDBNull(row[i]))
          {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string value = row[i].ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (value!.Contains(','))
            {
              value = String.Format("\"{0}\"", value);
              sw.Write(value);
            }
            else
            {
              sw.Write(row[i].ToString());
            }
          }
          if (i < input.Columns.Count - 1)
          {
            sw.Write(",");
          }
        }
        sw.Write(sw.NewLine);
      }
      sw.Close();

      return stream.ToArray();

    }

  }
}
