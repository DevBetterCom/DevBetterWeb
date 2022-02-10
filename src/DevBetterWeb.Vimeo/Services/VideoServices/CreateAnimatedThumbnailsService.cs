using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Interfaces;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;
 
public class CreateAnimatedThumbnailsService
{
  private readonly GetAnimatedThumbnailService _getAnimatedThumbnailService;
  private readonly GetStatusAnimatedThumbnailService _getStatusAnimatedThumbnailService;
  private readonly AddAnimatedThumbnailsToVideoService _addAnimatedThumbnailsToVideoService;
  private readonly GetVideoService _getVideoService;
  private readonly ILogger<CreateAnimatedThumbnailsService> _logger;
  private readonly ISleepService _sleepService;

  public CreateAnimatedThumbnailsService(
    GetAnimatedThumbnailService getAnimatedThumbnailService, 
    GetStatusAnimatedThumbnailService getStatusAnimatedThumbnailService, 
    AddAnimatedThumbnailsToVideoService addAnimatedThumbnailsToVideoService, 
    GetVideoService getVideoService, 
    ILogger<CreateAnimatedThumbnailsService> logger,
    ISleepService sleepService)
  {
    _getAnimatedThumbnailService = getAnimatedThumbnailService;
    _getStatusAnimatedThumbnailService = getStatusAnimatedThumbnailService;
    _addAnimatedThumbnailsToVideoService = addAnimatedThumbnailsToVideoService;
    _getVideoService = getVideoService;
    _logger = logger;
    _sleepService = sleepService;
  }

  public async Task<AnimatedThumbnailsResponse> ExecuteAsync(long videoId, 
    CancellationToken cancellationToken = default)
  {
    return await CreateAnimatedThumbnails(videoId);
  }

  private async Task<AnimatedThumbnailsResponse> CreateAnimatedThumbnails(long videoId)
  {
    Video video = new Video();
    while (video == null || video.Status != "available")
    {
      _sleepService.Sleep(20 * 1000);
      var response = await _getVideoService.ExecuteAsync(videoId.ToString());
      video = response.Data;
    }

    var startAnimation = GetRandomStart(video.Duration > 6 ? video.Duration : 0);
    var addAnimatedThumbnailsToVideoRequest = new AddAnimatedThumbnailsToVideoRequest(videoId, startAnimation, video.Duration >= 6 ? 6 : video.Duration);
    addAnimatedThumbnailsToVideoRequest.VideoId = null;
    var addAnimatedThumbnailsToVideoResult = await _addAnimatedThumbnailsToVideoService.ExecuteAsync(addAnimatedThumbnailsToVideoRequest);
    var pictureId = addAnimatedThumbnailsToVideoResult?.Data?.PictureId;
    if (string.IsNullOrEmpty(pictureId))
    {
      _logger.LogError($"Creating Animated Thumbnails Error!");
      _logger.LogError($"StatusCode: {addAnimatedThumbnailsToVideoResult?.Code}");
      _logger.LogError($"Error: {addAnimatedThumbnailsToVideoResult?.Text}");
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

      _sleepService.Sleep(5 * 1000);
    }
    var getAnimatedThumbnailResult = await _getAnimatedThumbnailService.ExecuteAsync(getStatusAnimatedThumbnailRequest);

    _logger.LogInformation($"Creating Animated Thumbnails Done!");

    return getAnimatedThumbnailResult.Data;
  }

  private int GetRandomStart(int max)
  {
    Random number = new Random();
    return number.Next(1, max);
  }
}
