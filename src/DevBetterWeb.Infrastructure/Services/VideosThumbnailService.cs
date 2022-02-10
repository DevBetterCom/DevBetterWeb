using System;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.Infrastructure.Services;

public class VideosThumbnailService : IVideosThumbnailService
{
  private readonly IAppLogger<VideosThumbnailService> _logger;
  private readonly IRepository<ArchiveVideo> _repositoryArchiveVideo;
  private readonly CreateAnimatedThumbnailsService _createAnimatedThumbnailsService;
  private readonly GetAllAnimatedThumbnailService _getAllAnimatedThumbnailService;

  public VideosThumbnailService(IAppLogger<VideosThumbnailService> logger, IRepository<ArchiveVideo> repositoryArchiveVideo, CreateAnimatedThumbnailsService createAnimatedThumbnailsService, GetAllAnimatedThumbnailService getAllAnimatedThumbnailService)
  {
    _logger = logger;
    _repositoryArchiveVideo = repositoryArchiveVideo;
    _createAnimatedThumbnailsService = createAnimatedThumbnailsService;
    _getAllAnimatedThumbnailService = getAllAnimatedThumbnailService;
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
