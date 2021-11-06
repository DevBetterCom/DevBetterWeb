using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class Domain
{
  [JsonPropertyName("allow_hd")]
  public string AllowHd { get; set; }
  [JsonPropertyName("domain")]
  public string DomainName { get; set; }
  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
