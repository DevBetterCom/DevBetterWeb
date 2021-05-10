namespace DevBetterWeb.Web.Pages.User
{
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

    public int GetDegrees()
    {
      var degrees = (int)(Percentage * ((double)360 / 100));

      return degrees;
    }

    public string GetDegreesForFirstPartialPie()
    { 
      if (Percentage <= 50)
      {
        var degreesStringUnder50 = $"{GetDegrees()}deg";
        return degreesStringUnder50;
      }
      var degrees = GetDegrees() - 180;
      var degreesString = $"{degrees}deg";
      return degreesString;
    }

    public string GetDegreesForSecondPartialPie()
    {
      if(Percentage <= 50)
      {
        return "0deg";
      }
      var degreesString = $"{GetDegrees()}deg";
      return degreesString;
    }

    public string GetOpacityForSecondPartialPie()
    {
      if(Percentage <= 50)
      {
        return "0";
      }
      return "1";
    }

  }
}
