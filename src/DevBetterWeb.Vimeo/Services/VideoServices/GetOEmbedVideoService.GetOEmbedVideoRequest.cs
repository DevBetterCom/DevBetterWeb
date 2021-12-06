namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetOEmbedVideoRequest
{
  public long VideoId { get; set; }
  public string Link { get; set; }

  public GetOEmbedVideoRequest(long videoId, string link)
  {
    VideoId = videoId;
    Link = link;
  }
}
