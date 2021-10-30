using DevBetterWeb.Vimeo.Models;
using static DevBetterWeb.Vimeo.Models.TextTrackType;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetUploadLinkTextTrackRequest
  {
    public string Uri { get; set; }
    public string Type { get; set; }
    public string Language { get; set; }
    public string Name { get; set; }

    public GetUploadLinkTextTrackRequest(string uri, TextTrackEnum textTrackType, string language="en", string name=null)
    {
      Uri = uri;
      Type = new TextTrackType(textTrackType).ToString();
      Language = language;
      Name = name;
    }
  }
}
