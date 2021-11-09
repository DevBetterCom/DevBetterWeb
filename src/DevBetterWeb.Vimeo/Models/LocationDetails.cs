using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class LocationDetails
{
  [JsonPropertyName("formatted_address")]
  public string FormattedAddress { get; set; }
  [JsonPropertyName("latitude")]
  public string Latitude { get; set; }
  [JsonPropertyName("longitude")]
  public string Longitude { get; set; }
  [JsonPropertyName("city")]
  public string City { get; set; }
  [JsonPropertyName("state")]
  public string State { get; set; }
  [JsonPropertyName("neighborhood")]
  public string Neighborhood { get; set; }
  [JsonPropertyName("sub_locality")]
  public string SubLocality { get; set; }
  [JsonPropertyName("state_iso_code")]
  public string StateIsoCode { get; set; }
  [JsonPropertyName("country")]
  public string Country { get; set; }
  [JsonPropertyName("country_iso_code")]
  public string CountryIsoCode { get; set; }
}
