using System;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.Infrastructure.Services;

public class VideosService : IVideosService
{
  private readonly IAppLogger<VideosService> _logger;
  private readonly IRepository<ArchiveVideo> _repositoryArchiveVideo;
  private readonly CreateAnimatedThumbnailsService _createAnimatedThumbnailsService;
  private readonly GetAllAnimatedThumbnailService _getAllAnimatedThumbnailService;
  private readonly GetVideoService _getVideoService;

  public VideosService(IAppLogger<VideosService> logger, IRepository<ArchiveVideo> repositoryArchiveVideo,
    CreateAnimatedThumbnailsService createAnimatedThumbnailsService, GetAllAnimatedThumbnailService getAllAnimatedThumbnailService, GetVideoService getVideoService)
  {
    _logger = logger;
    _repositoryArchiveVideo = repositoryArchiveVideo;
    _createAnimatedThumbnailsService = createAnimatedThumbnailsService;
    _getAllAnimatedThumbnailService = getAllAnimatedThumbnailService;
    _getVideoService = getVideoService;
  }

  public async Task UpdateVideosThumbnail(AppendOnlyStringList messages)
  {
    var spec = new ArchiveVideoWithoutThumbnailSpec();
    var videos = await _repositoryArchiveVideo.ListAsync(spec);
    foreach (var video in videos)
    {
      if (video?.VideoId == null)
      {
        continue;
      }
      try
      {
        var response = await _getVideoService.ExecuteAsync(video.VideoId);
        if (response?.Data == null)
        {
          continue;
        }
        var existThumbsResponse = await _getAllAnimatedThumbnailService.ExecuteAsync(new GetAnimatedThumbnailRequest(long.Parse(video.VideoId), null));
        if (existThumbsResponse.Data.Total <= 0)
        {
          var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(long.Parse(video.VideoId));
          if (getAnimatedThumbnailResult == null)
          {
            continue;
          }
          video.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
        }
        else
        {
          video.AnimatedThumbnailUri = existThumbsResponse.Data.Data.FirstOrDefault()?.AnimatedThumbnailUri;
        }

        await _repositoryArchiveVideo.UpdateAsync(video);
        messages?.Append($"Video {video.VideoId} updated with Thumbnails.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error on Thumbnails for Video {video.VideoId}: {ex.Message}");
      }
    }
  }

  public async Task DeleteVideosNotExistOnVimeo(AppendOnlyStringList messages)
  {
    var spec = new ArchiveVideoWithoutThumbnailSpec();
    var videos = await _repositoryArchiveVideo.ListAsync(spec);
    foreach (var video in videos)
    {
      if (video?.VideoId == null)
      {
        continue;
      }
      try
      {
        var response = await _getVideoService.ExecuteAsync(video.VideoId);
        if (response?.Data == null)
        {
          await _repositoryArchiveVideo.DeleteAsync(video);
        }

        messages?.Append($"Video {video.VideoId} deleted as it does not exist no vimeo.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error on Thumbnails for Video {video.VideoId}: {ex.Message}");
      }
    }
  }

  public async Task UpdateVideosThumbnailWithoutMessages()
  {
    var spec = new ArchiveVideoWithoutThumbnailSpec();
    var videos = await _repositoryArchiveVideo.ListAsync(spec);
    foreach (var video in videos)
    {
      if (video?.VideoId == null)
      {
        continue;
      }
      try
      {
        var response = await _getVideoService.ExecuteAsync(video.VideoId);
        if (response?.Data == null)
        {
          continue;
        }
        var existThumbsResponse = await _getAllAnimatedThumbnailService.ExecuteAsync(new GetAnimatedThumbnailRequest(long.Parse(video.VideoId), null));
        if (existThumbsResponse.Data.Total <= 0)
        {
          var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(long.Parse(video.VideoId));
          if (getAnimatedThumbnailResult == null)
          {
            continue;
          }
          video.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
        }
        else
        {
          video.AnimatedThumbnailUri = existThumbsResponse.Data.Data.FirstOrDefault()?.AnimatedThumbnailUri;
        }
        await _repositoryArchiveVideo.UpdateAsync(video);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error on Thumbnails for Video {video.VideoId}: {ex.Message}");
      }
    }
  }
}
