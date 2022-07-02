using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.WpfUploader.ApiClients;
using DevBetterWeb.WpfUploader.Models;
using MediaInfo;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DevBetterWeb.WpfUploader.Services;

// TODO: Refactor - this is way too big and has too many responsibilities and abstraction levels in it.
public class UploaderService
{
  private const string ALL_FILES = "*.*";
  private const string MP4_FILES = "*.mp4";
  private const string MD_FILES = "*.md";
  private const string SRT_FILES = ".srt";
  private const string VTT_FILES = ".vtt";
  private const string API_KEY_NAME = "API_KEY";

  private readonly UploadVideoService _uploadVideoService;
  private readonly GetAllVideosService _getAllVideosService;
  private readonly GetStatusAnimatedThumbnailService _getStatusAnimatedThumbnailService;
  private readonly GetAnimatedThumbnailService _getAnimatedThumbnailService;
  private readonly AddAnimatedThumbnailsToVideoService _addAnimatedThumbnailsToVideoService;
  private readonly UploadSubtitleToVideoService _uploadSubtitleToVideoService;
  private readonly GetVideoService _getVideoService;
  private readonly DeleteVideoService _deleteVideoService;
  private readonly ILogger<UploaderService> _logger;
  private readonly AddVideoInfo _addVideoInfo;
  private readonly UpdateVideoThumbnails _updateVideoThumbnails;
  private readonly DeleteVideo _deleteVideo;
  private readonly ConfigInfo _configInfo;

  public UploaderService(ConfigInfo configInfo, HttpService httpService,
    UploadVideoService uploadVideoService,
    GetAllVideosService getAllVideosService,
    GetStatusAnimatedThumbnailService getStatusAnimatedThumbnailService,
    GetAnimatedThumbnailService getAnimatedThumbnailService,
    AddAnimatedThumbnailsToVideoService addAnimatedThumbnailsToVideoService,
    UploadSubtitleToVideoService uploadSubtitleToVideoService,
    DeleteVideoService deleteVideoService,
    GetVideoService getVideoService,
    ILogger<UploaderService> logger)
  {
    _configInfo = configInfo;
    httpService.SetAuthorization(_configInfo.Token);
    _uploadVideoService = uploadVideoService;
    _getAllVideosService = getAllVideosService;
    _addAnimatedThumbnailsToVideoService = addAnimatedThumbnailsToVideoService;
    _uploadSubtitleToVideoService = uploadSubtitleToVideoService;
    _getAnimatedThumbnailService = getAnimatedThumbnailService;
    _getStatusAnimatedThumbnailService = getStatusAnimatedThumbnailService;
    _getVideoService = getVideoService;
    _deleteVideoService = deleteVideoService;
    _logger = logger;
    var clientHttp = new System.Net.Http.HttpClient();
    clientHttp.BaseAddress = new Uri(_configInfo.ApiLink);
    clientHttp.DefaultRequestHeaders.Add(API_KEY_NAME, _configInfo.ApiKey);

    var videoInfoHttpService = new HttpService(clientHttp);
    _addVideoInfo = new AddVideoInfo(videoInfoHttpService);
    _updateVideoThumbnails = new UpdateVideoThumbnails(videoInfoHttpService);
    _deleteVideo = new DeleteVideo(videoInfoHttpService);
  }

  public async Task<bool> DeleteVimeoVideoAsync(string vimeoId)
  {
    _logger.LogInformation("DeleteVimeoVideoAsync Started");

    var deleteResponse = await _deleteVideo.ExecuteAsync(vimeoId);
    var responseCode = deleteResponse?.Code;
    _logger.LogDebug($"Delete Response Code: {responseCode}");

    if (responseCode != HttpStatusCode.OK)
    {
      _logger.LogInformation($"{vimeoId} Is Not Delete!");
      _logger.LogError($"Delete Response Code: {responseCode}");
      _logger.LogError($"Delete Response Text: {deleteResponse.Text}");
      return false;
    }

    _logger.LogInformation($"{vimeoId} Is Deleted.");
    return true;
  }


  public async Task UpdateAnimatedThumbnailsAsync(string vimeoId)
  {
    _logger.LogInformation("UpdateAnimatedThumbnailsAsync Started");

    var response = await _getVideoService.ExecuteAsync(vimeoId);
    if (response?.Code != HttpStatusCode.OK)
    {
      _logger.LogInformation("Video Does Not Exist on Vimeo!");
      _logger.LogError($"{vimeoId} Update Animated Thumbnails Error!");
      _logger.LogError($"Error: {response.Text}");
      return;
    }

    var archiveVideo = new ArchiveVideo
    {
      VideoId = vimeoId
    };

    var getAnimatedThumbnailResult = await CreateAnimatedThumbnails(long.Parse(vimeoId));
    _logger.LogDebug($"AnimatedThumbnailUri: {getAnimatedThumbnailResult.AnimatedThumbnailUri}");

    archiveVideo.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;

    var updateVideoThumbnailsResponse = await _updateVideoThumbnails.ExecuteAsync(archiveVideo);
    if (updateVideoThumbnailsResponse == null || updateVideoThumbnailsResponse.Code != System.Net.HttpStatusCode.OK)
    {
      _logger.LogError($"{vimeoId} Update Animated Thumbnails Error!");
      _logger.LogError($"Error: {updateVideoThumbnailsResponse.Text}");
      return;
    }

    _logger.LogInformation($"{vimeoId} Is Updated.");
  }

  public async Task<List<Video>> LoadVideosAsync(string folderToUpload)
  {
    _logger.LogInformation("LoadVideosAsync Started");

    _logger.LogDebug($"Getting existing videos from devBetter API");
    var allExistingVideos = await GetExistingVideosAsync();
    _logger.LogDebug($"Found {allExistingVideos.Count} videos in devBetter API.");

    var localVideos = GetVideos(folderToUpload, allExistingVideos);
    _logger.LogInformation($"Found {localVideos.Count} videos in {folderToUpload}.");

    var resultVideos = new List<Video>();
    foreach (var video in localVideos)
    {
      var vimeoVideo = allExistingVideos.FirstOrDefault(x => x.Name.ToLower() == video.Name.ToLower());
      if (vimeoVideo != null)
      {
        _logger.LogWarning($"{video.Name} already exists on vimeo.");

        video.Link = vimeoVideo.Link;
      }

      resultVideos.Add(video);
    }

    foreach (var video in allExistingVideos)
    {
      var isExist = localVideos.Any(x => x.Name.ToLower() == video.Name.ToLower());
      if (!isExist)
      {
        resultVideos.Add(video);
      }
    }

    return resultVideos;
  }

  public async Task SyncAsync(string folderToUpload)
  {
    _logger.LogInformation("SyncAsync Started");

    _logger.LogDebug($"Getting existing videos from devBetter API");
    var allExistingVideos = await GetExistingVideosAsync();
    _logger.LogDebug($"Found {allExistingVideos.Count} videos in devBetter API.");

    var videosToUpload = GetVideos(folderToUpload, allExistingVideos);
    _logger.LogInformation($"Found {videosToUpload.Count} videos in {folderToUpload}.");
    foreach (var video in videosToUpload)
    {
      var vimeoVideo = allExistingVideos.FirstOrDefault(x => x.Name.ToLower() == video.Name.ToLower());
      if (vimeoVideo != null)
      {
        _logger.LogWarning($"{video.Name} already exists on vimeo.");
        _logger.LogInformation($"{video.Name} updating video info.");
        await UpdateVideoInfoAsync(video, long.Parse(vimeoVideo.Id), false);

        var uploadSubtitleToVideoRequest = new UploadSubtitleToVideoRequest(vimeoVideo.Id, video.Subtitle, "en");
        _ = await _uploadSubtitleToVideoService.ExecuteAsync(uploadSubtitleToVideoRequest);

        continue;
      }

      _logger.LogInformation($"Starting Uploading {video.Name}");

      video.Data = await File.ReadAllBytesAsync(video.LocalFullPath);

      // TODO: Would be good to have some progress indicator here...
      await UploadVideoAsync(video);
      video.Data = null;
    }
  }

  private async Task UploadVideoAsync(Video video)
  {
    if (string.IsNullOrEmpty(video.Description))
    {
      _logger.LogWarning($"{video.Name} has no associated MD file(s)...");
    }
    var request = new UploadVideoRequest(ServiceConstants.ME, video.Data, video, _configInfo.ApiLink
      .Replace("https://", string.Empty)
      .Replace("http://", string.Empty)
      .Replace("/", string.Empty));

    request.FileData = video.Data;

    var response = await _uploadVideoService.ExecuteAsync(request);
    var videoId = response.Data;
    if (videoId > 0)
    {
      if (string.IsNullOrEmpty(video.Subtitle))
      {
        _logger.LogWarning($"{video.Name} has no associated Subtitle file(s)...");
      }
      else
      {
        var uploadSubtitleToVideoRequest = new UploadSubtitleToVideoRequest(videoId.ToString(), video.Subtitle, "en");
        _ = await _uploadSubtitleToVideoService.ExecuteAsync(uploadSubtitleToVideoRequest);
      }

      _logger.LogInformation($"{video.Name} Uploaded!");

      await UpdateVideoInfoAsync(video, videoId, false);
    }
    else
    {
      _logger.LogError($"{video.Name} Upload Error!");
    }
  }

  private int GetRandomStart(int max)
  {
    Random number = new Random();
    return number.Next(1, max);
  }

  private async Task<bool> UpdateVideoInfoAsync(Video video, long videoId, bool createThumbnails = true)
  {
    var archiveVideo = new ArchiveVideo
    {
      Title = video.Name,
      DateCreated = new DateTimeOffset(video.CreatedTime.ToUniversalTime()),
      DateUploaded = DateTimeOffset.UtcNow,
      Duration = video.Duration,
      VideoId = videoId.ToString(),
      Password = video.Password,
      Description = video.Description,
      VideoUrl = video.Link
    };

    if (createThumbnails)
    {
      var getAnimatedThumbnailResult = await CreateAnimatedThumbnails(videoId);
      archiveVideo.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
    }

    var videoInfoResponse = await _addVideoInfo.ExecuteAsync(archiveVideo);
    if (videoInfoResponse == null || videoInfoResponse.Code != System.Net.HttpStatusCode.OK)
    {
      _logger.LogError($"{video.Name} - {videoId} Add/Update info Error!");
      _logger.LogError($"Error: {videoInfoResponse.Text}");
      return false;
    }

    _logger.LogInformation($"{video.Name} - {videoId} Add/Update info Done.");
    return true;
  }

  private async Task<AnimatedThumbnailsResponse> CreateAnimatedThumbnails(long videoId)
  {
    _logger.LogInformation($"Creating Animated Thumbnails");

    Video video = new Video();
    while (video.Status != "available")
    {
      Thread.Sleep(20 * 1000);
      var response = await _getVideoService.ExecuteAsync(videoId.ToString());
      if (response?.Data == null)
      {
        _logger.LogError($"Video does not exist on vimeo!");

        return null;
      }

      video = response.Data;
    }

    var startAnimation = GetRandomStart(video.Duration > 6 ? video.Duration : 0);
    var addAnimatedThumbnailsToVideoRequest = new AddAnimatedThumbnailsToVideoRequest(videoId, startAnimation, video.Duration >= 6 ? 6 : video.Duration);
    var addAnimatedThumbnailsToVideoResult = await _addAnimatedThumbnailsToVideoService.ExecuteAsync(addAnimatedThumbnailsToVideoRequest);
    var pictureId = addAnimatedThumbnailsToVideoResult?.Data?.PictureId;
    if (string.IsNullOrEmpty(pictureId))
    {
      _logger.LogError($"Creating Animated Thumbnails Error!");
      return null;
    }

    var statusAnimatedThumbnails = string.Empty;
    var getStatusAnimatedThumbnailRequest = new GetAnimatedThumbnailRequest(videoId, pictureId);
    while (statusAnimatedThumbnails != "completed")
    {
      var statusResult = await _getStatusAnimatedThumbnailService.ExecuteAsync(getStatusAnimatedThumbnailRequest);
      if (statusResult.Code == System.Net.HttpStatusCode.InternalServerError || statusResult.Code == System.Net.HttpStatusCode.Unauthorized || statusResult.Code == System.Net.HttpStatusCode.NotFound)
      {
        statusAnimatedThumbnails = string.Empty;
      }
      else
      {
        statusAnimatedThumbnails = statusResult.Data.Status;
      }

      Thread.Sleep(5 * 1000);
    }
    var getAnimatedThumbnailResult = await _getAnimatedThumbnailService.ExecuteAsync(getStatusAnimatedThumbnailRequest);

    _logger.LogInformation($"Creating Animated Thumbnails Done!");

    return getAnimatedThumbnailResult.Data;
  }

  private async Task<List<Video>> GetExistingVideosAsync()
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

  private List<Video> GetVideos(string folderPath, List<Video> existingVideos)
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

    if (videosPaths.Length > 0)
    {
      foreach (var videoPath in videosPaths)
      {

        var video = new Video();
        video.LocalFullPath = videoPath;

        var mdFilePath = mdsPaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(videoPath.ToLower().Trim()));
        var description = string.IsNullOrEmpty(mdFilePath) ? string.Empty : File.ReadAllText(mdFilePath);

        var subtitlePath = subtitlePaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(videoPath.ToLower().Trim()));
        var subtitle = string.IsNullOrEmpty(subtitlePath) ? string.Empty : File.ReadAllText(subtitlePath);

        var name = Path.GetFileNameWithoutExtension(videoPath);
        _logger.LogDebug($"Update {name} Video MD, Subtitle and Mp4 information");

        var mediaInfo = new MediaInfoWrapper(videoPath);
        if (mediaInfo.Size <= 0)
        {
          continue;
        }
        
        video
          .SetCreatedTime(mediaInfo.Tags.EncodedDate)
          .SetDuration(mediaInfo.Duration)
          .SetName(Path.GetFileNameWithoutExtension(videoPath))
          .SetDescription(description)
          .SetSubtitle(subtitle);

        video
          .SetEmbedProtecedPrivacy()
          .SetEmbed();
        result.Add(video);
      }
    }
    else if (mdsPaths.Length > 0)
    {
      foreach (var path in mdsPaths)
      {
        var mdFilePath = mdsPaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(path.ToLower().Trim()));
        var name = Path.GetFileNameWithoutExtension(mdFilePath);
        if (string.IsNullOrEmpty(name))
        {
          continue;
        }

        _logger.LogDebug($"Update {name} Video MD");

        var video = existingVideos.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        var description = string.IsNullOrEmpty(mdFilePath) ? string.Empty : File.ReadAllText(mdFilePath);

        video
          .SetDescription(description);

        result.Add(video);
      }
    }
    else if (subtitlePaths.Length > 0)
    {
      foreach (var path in subtitlePaths)
      {
        var subtitlePath = subtitlePaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(path.ToLower().Trim()));
        var name = Path.GetFileNameWithoutExtension(subtitlePath);
        if (string.IsNullOrEmpty(name))
        {
          continue;
        }

        _logger.LogDebug($"Update {name} Video Subtitle");

        var video = existingVideos.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        var subtitle = string.IsNullOrEmpty(subtitlePath) ? string.Empty : File.ReadAllText(subtitlePath);

        video
          .SetSubtitle(subtitle);

        result.Add(video);
      }
    }


    return result;
  }
}
