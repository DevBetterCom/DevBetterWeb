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
}
