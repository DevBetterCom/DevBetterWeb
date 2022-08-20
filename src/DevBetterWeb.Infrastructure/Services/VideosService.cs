using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
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
  private readonly DeleteVideoService _deleteVideoService;
  private readonly GetVideoService _getVideoService;
  private readonly GetPagedVideosService _getPagedVideosService;
  public readonly IVideosCacheService _videosCacheService;

  public VideosService(IAppLogger<VideosService> logger, IRepository<ArchiveVideo> repositoryArchiveVideo,
    CreateAnimatedThumbnailsService createAnimatedThumbnailsService, GetAllAnimatedThumbnailService getAllAnimatedThumbnailService,
    DeleteVideoService deleteVideoService, GetVideoService getVideoService, GetPagedVideosService getPagedVideosService, IVideosCacheService videosCacheService)
  {
    _logger = logger;
    _repositoryArchiveVideo = repositoryArchiveVideo;
    _createAnimatedThumbnailsService = createAnimatedThumbnailsService;
    _getAllAnimatedThumbnailService = getAllAnimatedThumbnailService;
    _deleteVideoService = deleteVideoService;
    _getVideoService = getVideoService;
    _getPagedVideosService = getPagedVideosService;
    _videosCacheService = videosCacheService;

  }

  public async Task UpdateVideosThumbnail(AppendOnlyStringList? messages, CancellationToken cancellationToken = default)
  {
    var spec = new ArchiveVideoWithoutThumbnailSpec();
    var videos = await _repositoryArchiveVideo.ListAsync(spec, cancellationToken);
    foreach (var video in videos)
    {
      try
      {
	      _ = long.TryParse(video.VideoId, out var videoId);
	      await UpdateVideoThumbnailsAsync(videoId, cancellationToken);

        messages?.Append($"Video {video.VideoId} updated with Thumbnails.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error on Thumbnails for Video {video.VideoId}: {ex.Message}");
      }
    }
  }

  public async Task UpdateVideosCache(AppendOnlyStringList? messages)
  {
	  try
	  {
		  await _videosCacheService.UpdateAllVideosAsync();
		  messages?.Append("Videos Cache Updated.");
	  }
	  catch (Exception ex)
	  {
		  _logger.LogError(ex, $"Error on Videos Cache Updated: {ex.Message}");
	  }
	}

	public async Task DeleteVideosNotExistOnVimeoFromDatabase(AppendOnlyStringList? messages)
  {
    var spec = new ArchiveVideoWithoutThumbnailSpec();
    var videos = await _repositoryArchiveVideo.ListAsync(spec);
    foreach (var video in videos)
    {
      try
      {
        var response = await _getVideoService.ExecuteAsync(video.VideoId);
        if (response?.Data != null && response.Data.IsPlayable == false)
        {
          await _deleteVideoService.ExecuteAsync(video.VideoId);
          messages?.Append($"Video {video.Id} deleted from vimeo as it does not exist on vimeo.");
        }
        if (response?.Data == null || response?.Data.IsPlayable == false)
        {
          await _repositoryArchiveVideo.DeleteAsync(video);
          messages?.Append($"Video {video.VideoId} deleted as it does not exist on vimeo.");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error on Delete Video {video.VideoId}: {ex.Message}");
      }
    }
  }

  public async Task DeleteVideosNotExistOnVimeoFromVimeo(AppendOnlyStringList? messages)
  {
    var request = new GetAllVideosRequest("me");
    var videosResponse = await _getPagedVideosService.ExecuteAsync(request);
    if (videosResponse?.Data?.Data == null)
    {
      return;
    }

    var videos = videosResponse.Data.Data;
    foreach (var video in videos)
    {
      try
      {
        if (video is { IsPlayable: false })
        {
          await _deleteVideoService.ExecuteAsync(video.Id);
          messages?.Append($"Video {video.Id} deleted from vimeo as it does not exist on vimeo.");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error on Delete Video from vimeo {video.Id}: {ex.Message}");
      }
    }
  }

  public async Task AddArchiveVideoInfo(ArchiveVideo archiveVideo, CancellationToken cancellationToken = default)
  {
	  var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId!);
	  var existVideo = await _repositoryArchiveVideo.FirstOrDefaultAsync(spec, cancellationToken);
	  if (existVideo == null)
	  {
		  var videoAddedEvent = new VideoAddedEvent(archiveVideo);
		  archiveVideo.Events.Add(videoAddedEvent);
		  _ = await _repositoryArchiveVideo.AddAsync(archiveVideo, cancellationToken);
	  }
	  else
	  {
		  existVideo.Description = archiveVideo.Description;
		  existVideo.Title = archiveVideo.Title;
		  existVideo.Duration = archiveVideo.Duration;
		  if (!string.IsNullOrEmpty(archiveVideo.AnimatedThumbnailUri))
		  {
			  existVideo.AnimatedThumbnailUri = archiveVideo.AnimatedThumbnailUri;
		  }

		  await _repositoryArchiveVideo.UpdateAsync(existVideo, cancellationToken);
	  }
	}

  public async Task<ArchiveVideo?> UpdateVideoThumbnailsAsync(long videoId, CancellationToken cancellationToken = default)
  {
	  if (videoId <= 0)
	  {
		  return null;
	  }

	  var response = await _getVideoService.ExecuteAsync(videoId.ToString(), cancellationToken);
	  if (response?.Data == null)
	  {
			return null;
		}

	  var existThumbsResponse = await _getAllAnimatedThumbnailService.ExecuteAsync(new GetAnimatedThumbnailRequest(videoId, null), cancellationToken);
	  var animatedThumbnailUri = string.Empty;

		if (existThumbsResponse.Data.Total <= 0)
	  {
		  var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(videoId, cancellationToken);
		  if (getAnimatedThumbnailResult == null)
		  {
				return null;
			}
		  animatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
	  }
	  else
	  {
		  animatedThumbnailUri = existThumbsResponse.Data.Data.FirstOrDefault()?.AnimatedThumbnailUri;
	  }
		var spec = new ArchiveVideoByVideoIdSpec(videoId.ToString());
	  var existVideo = await _repositoryArchiveVideo.FirstOrDefaultAsync(spec, cancellationToken);
	  if (existVideo == null)
	  {
		  return null;
	  }

	  existVideo.AnimatedThumbnailUri = animatedThumbnailUri;
  
	  await _repositoryArchiveVideo.UpdateAsync(existVideo, cancellationToken);

	  return existVideo;
  }
}
