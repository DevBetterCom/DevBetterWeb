using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using MediaInfo;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DevBetterWeb.UploaderApp
{
  // TODO: Refactor - this is way too big and has too many responsibilities and abstraction levels in it.
  public class UploaderService
  {
    private const string MP4_FILES = "*.mp4";
    private const string MD_FILES = "*.md";
    private const string API_KEY_NAME = "API_KEY";

    private readonly UploadVideoService _uploadVideoService;
    private readonly GetAllVideosService _getAllVideosService;
    private readonly GetStatusAnimatedThumbnailService _getStatusAnimatedThumbnailService;
    private readonly GetAnimatedThumbnailService _getAnimatedThumbnailService;
    private readonly AddAnimatedThumbnailsToVideoService _addAnimatedThumbnailsToVideoService;
    private readonly GetVideoService _getVideoService;
    private readonly ILogger<UploaderService> _logger;
    private readonly AddVideoInfo _addVideoInfo;
    private readonly ConfigInfo _configInfo;

    public UploaderService(ConfigInfo configInfo, HttpService httpService,
      UploadVideoService uploadVideoServicestring,
      GetAllVideosService getAllVideosService,
      GetStatusAnimatedThumbnailService getStatusAnimatedThumbnailService,
      GetAnimatedThumbnailService getAnimatedThumbnailService,
      AddAnimatedThumbnailsToVideoService addAnimatedThumbnailsToVideoService,
      GetVideoService getVideoService,
      ILogger<UploaderService> logger)
    {
      _configInfo = configInfo;
      httpService.SetAuthorization(_configInfo.Token);
      _uploadVideoService = uploadVideoServicestring;
      _getAllVideosService = getAllVideosService;
      _addAnimatedThumbnailsToVideoService = addAnimatedThumbnailsToVideoService;
      _getAnimatedThumbnailService = getAnimatedThumbnailService;
      _getStatusAnimatedThumbnailService = getStatusAnimatedThumbnailService;
      _getVideoService = getVideoService;
      _logger = logger;
      var clientHttp = new System.Net.Http.HttpClient();
      clientHttp.BaseAddress = new Uri(_configInfo.ApiLink);
      clientHttp.DefaultRequestHeaders.Add(API_KEY_NAME, _configInfo.ApiKey);

      var videoInfoHttpService = new HttpService(clientHttp);
      _addVideoInfo = new AddVideoInfo(videoInfoHttpService);
    }

    public async Task SyncAsync(string folderToUpload)
    {
      _logger.LogInformation("SyncAsync Started");

      _logger.LogDebug($"Getting existing videos from devBetter API");
      var allExistingVideos = await GetExistingVideosAsync();
      _logger.LogDebug($"Found {allExistingVideos.Count} videos in devBetter API.");

      var videosToUpload = GetVideos(folderToUpload);
      _logger.LogInformation($"Found {videosToUpload.Count} videos in {folderToUpload}.");
      foreach (var video in videosToUpload)
      {
        if (allExistingVideos.Any(x => x.Name.ToLower() == video.Name.ToLower()))
        {
          _logger.LogWarning($"{video.Name} already exists - skipping.");
          continue;
        }

        _logger.LogInformation($"Starting Uploading {video.Name}");
        // TODO: Would be good to have some progress indicator here...
        await UploadVideoAsync(video);
      }
    }

    private async Task UploadVideoAsync(Video video)
    {
      if (string.IsNullOrEmpty(video.Description))
      {
        _logger.LogWarning($"{video.Name} has no associated MD file(s)...");
      }
      var request = new UploadVideoRequest(ServiceConstants.ME, video.Data, video, _configInfo.ApiLink
        .Replace("https://", String.Empty)
        .Replace("http://", String.Empty)
        .Replace("/", String.Empty));

      request.FileData = video.Data;

      var response = await _uploadVideoService.ExecuteAsync(request);
      var videoId = response.Data;
      if (videoId > 0)
      {
        _logger.LogInformation($"{video.Name} Uploaded!");

        await UpdateVideoInfoAsync(video, videoId);
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

    private async Task<bool> UpdateVideoInfoAsync(Video video, long videoId)
    {
      var archiveVideo = new ArchiveVideo
      {
        Title = video.Name,
        DateCreated = video.CreatedTime,
        DateUploaded = DateTimeOffset.UtcNow,
        Duration = video.Duration,
        VideoId = videoId.ToString(),
        Password = video.Password,
        Description = video.Description,
        VideoUrl = video.Link
      };

      var getAnimatedThumbnailResult = await CreateAnimatedThumbnails(videoId);
      archiveVideo.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
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
      while(video == null || video.Status != "available")
      {
        Thread.Sleep(20 * 1000);
        var response = await _getVideoService.ExecuteAsync(videoId.ToString());
        video = response.Data;
      }

      var startAnimation = GetRandomStart(video.Duration>6? video.Duration:0);
      var addAnimatedThumbnailsToVideoRequest = new AddAnimatedThumbnailsToVideoRequest(videoId, startAnimation, video.Duration>=6?6: video.Duration);
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
        }else
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
      var getAllVideosRequest = new GetAllVideosRequest(ServiceConstants.ME);
      var allExistingVideos = await _getAllVideosService.ExecuteAsync(getAllVideosRequest);

      if(allExistingVideos.Code != System.Net.HttpStatusCode.OK)
      {
        throw new Exception($"Non-successful status code: {allExistingVideos.Code}");
      }
      return allExistingVideos.Data.Data;
    }

    private List<Video> GetVideos(string folderPath)
    {
      var result = new List<Video>();
      if (!Directory.Exists(folderPath))
      {
        return result;
      }

      string[] videosPaths = Directory.GetFiles(folderPath, MP4_FILES, SearchOption.AllDirectories);
      if (videosPaths == null)
      {
        return result;
      }

      string[] mdsPaths = Directory.GetFiles(folderPath, MD_FILES, SearchOption.AllDirectories);
      if (mdsPaths == null)
      {
        return result;
      }
      
      foreach (var videoPath in videosPaths)
      {
        var video = new Video();

        var mdFilePath = mdsPaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(videoPath.ToLower().Trim()));
        var description = string.IsNullOrEmpty(mdFilePath) ? string.Empty : File.ReadAllText(mdFilePath);

        var mediaInfo = new MediaInfoWrapper(videoPath);
        video
          .SetCreatedTime(mediaInfo.Tags.EncodedDate)
          .SetDuration(mediaInfo.Duration)
          .SetDuration(mediaInfo.Duration)
          .SetName(Path.GetFileNameWithoutExtension(videoPath))
          .SetDescription(description);

        video.Data = File.ReadAllBytes(videoPath);
        if (video.Data == null || video.Data.Length <= 0)
        {
          continue;
        }
        video
          .SetEmbedProtecedPrivacy()
          .SetEmbed();
        result.Add(video);
      }

      return result;
    }
  }
}
