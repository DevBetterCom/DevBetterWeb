using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class VideoForCreateResumableLink
{

	[JsonPropertyName("upload")]
  public Upload Upload { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
	
  public string VideoId => Uri.Split("/")[Uri.Split("/").Length - 1];
}
