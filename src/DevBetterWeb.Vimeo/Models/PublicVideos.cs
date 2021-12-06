using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class PublicVideos
{
  [JsonPropertyName("total")]
  public int Total { get; set; }
}
