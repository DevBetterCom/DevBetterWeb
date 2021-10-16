using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using MediaInfo;

namespace DevBetterWeb.UploaderApp
{
  public class UploaderService
  {
    private static string MP4_FILES = "*.mp4";
    private static string MD_FILES = "*.md";
    private static string API_KEY_NAME = "ApiKey";

    private readonly UploadVideoService _uploadVideoService;
    private readonly GetAllVideosService _getAllVideosService;
    private readonly GetStatusAnimatedThumbnailService _getStatusAnimatedThumbnailService;
    private readonly GetAnimatedThumbnailService _getAnimatedThumbnailService;
    private readonly AddAnimatedThumbnailsToVideoService _addAnimatedThumbnailsToVideoService;
    private readonly GetVideoService _getVideoService;
    private readonly AddVideoInfo _addVideoInfo;

    public UploaderService(ConfigInfo configInfo, HttpService httpService, UploadVideoService uploadVideoServicestring,
      GetAllVideosService getAllVideosService, GetStatusAnimatedThumbnailService getStatusAnimatedThumbnailService, GetAnimatedThumbnailService getAnimatedThumbnailService,
      AddAnimatedThumbnailsToVideoService addAnimatedThumbnailsToVideoService, GetVideoService getVideoService)
    {
      httpService.SetAuthorization(configInfo.Token);
      _uploadVideoService = uploadVideoServicestring;
      _getAllVideosService = getAllVideosService;
      _addAnimatedThumbnailsToVideoService = addAnimatedThumbnailsToVideoService;
      _getAnimatedThumbnailService = getAnimatedThumbnailService;
      _getStatusAnimatedThumbnailService = getStatusAnimatedThumbnailService;
      _getVideoService = getVideoService;

      var clientHttp = new System.Net.Http.HttpClient();
      clientHttp.BaseAddress = new Uri(configInfo.ApiLink);
      clientHttp.DefaultRequestHeaders.Add(API_KEY_NAME, configInfo.ApiKey);

      var videoInfoHttpService = new HttpService(clientHttp);
      _addVideoInfo = new AddVideoInfo(videoInfoHttpService);
    }

    public async Task SyncAsync(string folderToUpload)
    {
      Console.WriteLine($"Getting Exist Videos.");

      var videos = GetVideos(folderToUpload);            

      var allExisVideos = await GetExistVideosAsync();
      Console.WriteLine($"Found {allExisVideos.Count} videos on Vimeo.");

      Console.WriteLine($"Getting Exist Videos Done.");

      foreach (var video in videos)
      {
        if (allExisVideos.Any(x => x.Name.ToLower() == video.Name.ToLower()))
        {
          Console.WriteLine($"{video.Name} already exists - skipping.");
          continue;
        }        

        Console.WriteLine($"Starting Uploading {video.Name}");
        if (string.IsNullOrEmpty(video.Description))
        {
          Console.WriteLine($"{video.Name} has no associated MD file(s)...");
        }
        var request = new UploadVideoRequest(ServiceConstants.ME, video.Data, video, "devbetter.com");
        request.FileData = video.Data;

        var response = await _uploadVideoService.ExecuteAsync(request);
        var videoId = response.Data;
        if (videoId > 0)
        {
          var archiveVideo = new ArchiveVideo
          {
            Title = video.Name,
            DateCreated = video.CreatedTime,
            DateUploaded = DateTimeOffset.UtcNow,
            Duration = video.Duration,
            VideoId = response.Data.ToString(),
            Password = video.Password,
            Description = video.Description,
            VideoUrl = video.Link
          };
          var getAnimatedThumbnailResult = await CreateAnimatedThumbnails(videoId);
          archiveVideo.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
          await _addVideoInfo.ExecuteAsync(archiveVideo);

          Console.WriteLine($"{video.Name} Uploaded!");
        }
        else
        {
          Console.WriteLine($"{video.Name} Upload Error!");
        }
      }
    }

    private async Task<AnimatedThumbnailsResponse> CreateAnimatedThumbnails(long videoId)
    {
      Console.WriteLine($"Creating Animated Thumbnails");

      Video video = new Video();
      while(video.Status != "available")
      {
        Thread.Sleep(20 * 1000);
        video = (await _getVideoService.ExecuteAsync(videoId.ToString())).Data;
      }

      var addAnimatedThumbnailsToVideoRequest = new AddAnimatedThumbnailsToVideoRequest(videoId, 0, 6);
      var addAnimatedThumbnailsToVideoResult = await _addAnimatedThumbnailsToVideoService.ExecuteAsync(addAnimatedThumbnailsToVideoRequest);
      var pictureId = addAnimatedThumbnailsToVideoResult?.Data?.PictureId;
      if (string.IsNullOrEmpty(pictureId))
      {
        Console.WriteLine($"Creating Animated Thumbnails Error!");
        return null;
      }

      var statusAnimatedThumbnails = string.Empty;
      var getStatusAnimatedThumbnailRequest = new GetAnimatedThumbnailRequest(videoId, pictureId);
      while (statusAnimatedThumbnails != "completed")
      {
        var statusResult = await _getStatusAnimatedThumbnailService.ExecuteAsync(getStatusAnimatedThumbnailRequest);

        statusAnimatedThumbnails = statusResult.Data.Status;
        Thread.Sleep(5 * 1000);
      }
      var getAnimatedThumbnailResult = await _getAnimatedThumbnailService.ExecuteAsync(getStatusAnimatedThumbnailRequest);

      Console.WriteLine($"Creating Animated Thumbnails Done!");

      return getAnimatedThumbnailResult.Data;
    }

    private async Task<List<Video>> GetExistVideosAsync()
    {      
      var getAllVideosRequest = new GetAllVideosRequest(ServiceConstants.ME);
      var allExisVideos = await _getAllVideosService.ExecuteAsync(getAllVideosRequest);

      return allExisVideos.Data.Data;
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
