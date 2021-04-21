namespace DevBetterWeb.Web.Pages.User
{
  public class MemberSubscriptionPercentViewModel
  {
    public string Percentage { get; set; }

    public MemberSubscriptionPercentViewModel(string percentage)
    {
      Percentage = percentage;
    }

    public void SetPercentage(string percentage)
    {
      Percentage = percentage;
    }
  }
}
