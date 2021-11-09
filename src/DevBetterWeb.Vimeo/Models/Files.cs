using System; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Files
{
  [JsonPropertyName("created_time")]
  public DateTime CreatedTime { get; set; }

  [JsonPropertyName("expires")]
  public DateTime Expires { get; set; }

  [JsonPropertyName("fps")]
  public int Fps { get; set; }

  [JsonPropertyName("height")]
  public int Height { get; set; }

  [JsonPropertyName("link")]
  public string Link { get; set; }

  [JsonPropertyName("log")]
  public Log Log { get; set; }

  [JsonPropertyName("md5")]
  public string Md5 { get; set; }

  [JsonPropertyName("public_name")]
  public string PublicName { get; set; }

  [JsonPropertyName("quality")]
  public string Quality { get; set; }

  [JsonPropertyName("size")]
  public int Size { get; set; }

  [JsonPropertyName("size_short")]
  public string SizeShort { get; set; }

  [JsonPropertyName("source_link")]
  public string SourceLink { get; set; }

  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("video_file_id")]
  public int VideoFileId { get; set; }

  [JsonPropertyName("width")]
  public int Width { get; set; }
}
