namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class AddDomainToVideoRequest
{
  public long VideoId { get; set; }
  public string Domain { get; set; }

  public AddDomainToVideoRequest(long videoId, string domain)
  {
    VideoId = videoId;
    Domain = domain;
  }
}
