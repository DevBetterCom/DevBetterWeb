using System; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Periodic
{
  [JsonPropertyName("free")]
  public long Free { get; set; }

  [JsonPropertyName("max")]
  public long Max { get; set; }

  [JsonPropertyName("reset_date")]
  public DateTime ResetDate { get; set; }

  [JsonPropertyName("used")]
  public long Used { get; set; }
}
