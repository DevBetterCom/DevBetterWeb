namespace DevBetterWeb.Web.Models;

public class UploadSubtitleRequest
{
  public string? VideoId { get; set; }
  public string? Subtitle { get; set; }
  public string? Language { get; set; }
}
