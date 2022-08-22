using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadTusRequest
{
	[JsonPropertyName("upload")]
	public UploadTus Upload { get; set; } = new UploadTus("0");

	public UploadTusRequest(int size)
	{
		Upload.Size = size.ToString();
	}

}
