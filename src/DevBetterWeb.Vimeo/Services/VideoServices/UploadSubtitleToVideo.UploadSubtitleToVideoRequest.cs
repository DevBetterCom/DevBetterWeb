using System.Text;
using DevBetterWeb.Vimeo.Helper;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadSubtitleToVideoRequest
{
  public string VideoId { get; set; }
  public byte[] SubtitleFile { get; set; }
  public string Language { get; set; }

  public UploadSubtitleToVideoRequest(string videoId, byte[] subtitleFile, string language = "en")
  {
    VideoId = videoId;
    SubtitleFile = ConvertToVttIfNeeded(subtitleFile);
    Language = language;
  }

  public UploadSubtitleToVideoRequest(string videoId, string subtitleFile, string language = "en")
  {
    VideoId = videoId;
    SubtitleFile = Encoding.ASCII.GetBytes(ConvertToVttIfNeeded(subtitleFile));
    Language = language;
  }

  private string ConvertToVttIfNeeded(string srtData)
  {
    if (srtData.Contains("WEBVTT"))
    {
      return srtData;
    }

    return SubtitleConverter.ConvertSrtToVtt(srtData);
  }

  private byte[] ConvertToVttIfNeeded(byte[] srtData)
  {
    if (Encoding.ASCII.GetString(srtData).Contains("WEBVTT"))
    {
      return srtData;
    }

    return Encoding.ASCII.GetBytes(SubtitleConverter.ConvertSrtToVtt(Encoding.ASCII.GetString(srtData)));
  }
}
