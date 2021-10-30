using System.Text;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class UploadSubtitleToVideoRequest
  {
    public string VideoId { get; set; }
    public byte[] VttFile { get; set; }
    public string Language { get; set; }

    public UploadSubtitleToVideoRequest(string videoId, byte[] vttFile, string language = "en")
    {
      VideoId = videoId;
      VttFile = vttFile;
      Language = language;
    }

    public UploadSubtitleToVideoRequest(string videoId, string vttFile, string language = "en")
    {
      VideoId = videoId;
      VttFile = Encoding.ASCII.GetBytes(vttFile);
      Language = language;
    }
  }
}
