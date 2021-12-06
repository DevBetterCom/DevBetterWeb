namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class ActiveTextTrackRequest
{
  public string Uri { get; set; }
  public bool Active { get; set; }

  public ActiveTextTrackRequest(string uri, bool active = true)
  {
    Uri = uri;
    Active = active;
  }
}
