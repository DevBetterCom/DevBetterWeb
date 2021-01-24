namespace DevBetterWeb.Web.Pages.User
{
  public class MapCoordinates
  {
    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string MemberFullName { get; set; }

    public bool IsClickedMember { get; set; }

    public MapCoordinates(decimal latitude, decimal longitude, string memberFullName)
    {
      Latitude = latitude;
      Longitude = longitude;
      MemberFullName = memberFullName;
    }
  }
}
