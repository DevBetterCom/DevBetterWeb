namespace DevBetterWeb.Web.Pages.User
{
  public class MapCoordinates
  {
    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public MapCoordinates(decimal latitude, decimal longitude)
    {
      Latitude = latitude;
      Longitude = longitude;
    }
  }
}
