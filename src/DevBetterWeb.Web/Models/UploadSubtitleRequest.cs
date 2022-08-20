namespace DevBetterWeb.Web.Models;

public class UploadSubtitleRequest
{
	public string VideoId { get; set; } = "0";
	public string? Subtitle { get; set; }
  public string? Language { get; set; }
}
