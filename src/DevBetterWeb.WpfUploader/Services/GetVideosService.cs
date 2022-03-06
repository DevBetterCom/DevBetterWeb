using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.WpfUploader.Models;
using MediaInfo;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.WpfUploader.Services;

public class GetVideosService
{
  private const string ALL_FILES = "*.*";
  private const string MP4_FILES = "*.mp4";
  private const string MD_FILES = "*.md";
  private const string SRT_FILES = ".srt";
  private const string VTT_FILES = ".vtt";

  private readonly GetAllVideosService _getAllVideosService;
  private readonly ILogger<GetVideosService> _logger;
  private readonly ConfigInfo _configInfo;

  public GetVideosService(
    ConfigInfo configInfo,
    HttpService httpService,
    GetAllVideosService getAllVideosService,
    ILogger<GetVideosService> logger)
  {
    _configInfo = configInfo;
    httpService.SetAuthorization(_configInfo.Token);
    _getAllVideosService = getAllVideosService;
    _logger = logger;
  }

  public async Task<List<Video>> GetAllVimeoVideosAsync()
  {
    HttpResponse<DataPaged<Video>> allVideosResponse;
    var videos = new List<Video>();

    var pageNumber = 1;
    do
    {
      var getAllRequest = new GetAllVideosRequest(ServiceConstants.ME, pageNumber);
      allVideosResponse = await _getAllVideosService.ExecuteAsync(getAllRequest);
      if (allVideosResponse != null && allVideosResponse.Data != null)
      {
        videos.AddRange(allVideosResponse.Data.Data);
      }
      pageNumber++;
    } while (allVideosResponse != null && allVideosResponse.Data != null);

    return videos;
  }

  public static List<Video> GetVideosFromFolder(string folderPath)
  {
    var result = new List<Video>();
    if (!Directory.Exists(folderPath))
    {
      return result;
    }

    string[] videosPaths = Directory.GetFiles(folderPath, MP4_FILES, SearchOption.AllDirectories);

    string[] mdsPaths = Directory.GetFiles(folderPath, MD_FILES, SearchOption.AllDirectories);

    string[] subtitlePaths = Directory.GetFiles(folderPath, ALL_FILES, SearchOption.AllDirectories)
      .Where(s => s.EndsWith(SRT_FILES) || s.EndsWith(VTT_FILES))
      .ToArray();

    foreach (var videoPath in videosPaths)
    {

      var video = new Video();

      var mdFilePath = mdsPaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(videoPath.ToLower().Trim()));
      var description = string.IsNullOrEmpty(mdFilePath) ? string.Empty : File.ReadAllText(mdFilePath);

      var subtitlePath = subtitlePaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(videoPath.ToLower().Trim()));
      var subtitle = string.IsNullOrEmpty(subtitlePath) ? string.Empty : File.ReadAllText(subtitlePath);

      var name = Path.GetFileNameWithoutExtension(videoPath);

      var mediaInfo = new MediaInfoWrapper(videoPath);
      video
        .SetCreatedTime(mediaInfo.Tags.EncodedDate)
        .SetDuration(mediaInfo.Duration)
        .SetName(name)
        .SetDescription(description)
        .SetSubtitle(subtitle)
        .SetEmbedProtecedPrivacy()
        .SetEmbed();
      result.Add(video);
    }

    return result;
  }
}
