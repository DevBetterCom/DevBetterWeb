using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadTus
{
	[JsonPropertyName("approach")]
	public string Approach { get; set; } = "tus";

	[JsonPropertyName("size")]
	public string Size { get; set; }

  public UploadTus(string size)
  {
	  Size = size;
  }
}
