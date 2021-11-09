using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Transcode
{
  [JsonPropertyName("status")]
  public string Status { get; set; }
}
