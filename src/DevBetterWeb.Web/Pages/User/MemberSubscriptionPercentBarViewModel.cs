namespace DevBetterWeb.Web.Pages.User
{
  public class MemberSubscriptionPercentBarViewModel
  {
    public int Percentage { get; set; }

    public MemberSubscriptionPercentBarViewModel(int percentage)
    {
      Percentage = percentage;
    }

    public string GetPercentageWithPercent()
    {
      var percentageWithPercent = $"{Percentage}%";
      return percentageWithPercent;
    }
  }

  public class MemberSubscriptionPercentCircleViewModel
  {
    public int Percentage { get; set; }

    public MemberSubscriptionPercentCircleViewModel(int percentage)
    {
      Percentage = percentage;
    }

    public string GetPercentageWithPercent()
    {
      var percentageWithPercent = $"{Percentage}%";
      return percentageWithPercent;
    }

    public string GetDegrees()
    {
      var degrees = (int)(Percentage * ((double)360 / 100));
      var degreesString = $"{degrees}deg";
      return degreesString;
    }

  }
}
