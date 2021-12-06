using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Privacy
{
  [JsonPropertyName("view")]
  public string View { get; set; }

  [JsonPropertyName("password")]
  public string Password { get; set; }

  [JsonPropertyName("add")]
  public bool Add { get; set; }

  [JsonPropertyName("comments")]
  public string Comments { get; set; }

  [JsonPropertyName("download")]
  public bool Download { get; set; }

  [JsonPropertyName("embed")]
  public string Embed { get; set; }
}
