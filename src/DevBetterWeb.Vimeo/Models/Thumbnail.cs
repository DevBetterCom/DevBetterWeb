using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class Thumbnail
{
  public int Duration { get; set; }
  [JsonPropertyName("file_format")]
  public string FileFormat { get; set; }
  [JsonPropertyName("file_size")]
  public int FileSize { get; set; }
  [JsonPropertyName("height")]
  public int Height { get; set; }
  [JsonPropertyName("is_downloadable")]
  public bool IsDownloadable { get; set; }
  [JsonPropertyName("link")]
  public string Link { get; set; }
  [JsonPropertyName("link_with_play_button")]
  public string LinkWithPlayButton { get; set; }
  [JsonPropertyName("profile_id")]
  public string ProfileId { get; set; }
  [JsonPropertyName("start_time")]
  public int StartTime { get; set; }
  [JsonPropertyName("uuid")]
  public string Uuid { get; set; }
  [JsonPropertyName("width")]
  public int Width { get; set; }
}
