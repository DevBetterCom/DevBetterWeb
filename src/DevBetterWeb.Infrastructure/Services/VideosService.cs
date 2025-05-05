using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using NimblePros.Vimeo.AnimatedThumbnailsServices;
using NimblePros.Vimeo.Services.VideoServices;
using NimblePros.Vimeo.VideoServices;

namespace DevBetterWeb.Infrastructure.Services;

public class VideosService : IVideosService
{
  private readonly IAppLogger<VideosService> _logger;
  private readonly IRepository<ArchiveVideo> _repositoryArchiveVideo;
  private readonly CreateSetAnimatedThumbnailsForVideoService _createAnimatedThumbnailsService;
  private readonly GetAllSetAnimatedThumbnailsForVideoService _getAllAnimatedThumbnailService;
  private readonly DeleteVideoService _deleteVideoService;
  private readonly GetVideoService _getVideoService;
  private readonly GetVideosUserAppearsService _getVideosUserAppearsService;

  public VideosService(IAppLogger<VideosService> logger, IRepository<ArchiveVideo> repositoryArchiveVideo,
		CreateSetAnimatedThumbnailsForVideoService createAnimatedThumbnailsService, GetAllSetAnimatedThumbnailsForVideoService getAllAnimatedThumbnailService,
    DeleteVideoService deleteVideoService, GetVideoService getVideoService, GetVideosUserAppearsService getVideosUserAppearsService)
  {
    _logger = logger;
    _repositoryArchiveVideo = repositoryArchiveVideo;
    _createAnimatedThumbnailsService = createAnimatedThumbnailsService;
    _getAllAnimatedThumbnailService = getAllAnimatedThumbnailService;
    _deleteVideoService = deleteVideoService;
    _getVideoService = getVideoService;
		_getVideosUserAppearsService = getVideosUserAppearsService;

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

  public async Task DeleteVideosNotExistOnVimeoFromDatabase(AppendOnlyStringList? messages)
  {
    var spec = new ArchiveVideoWithoutThumbnailSpec();
    var videos = await _repositoryArchiveVideo.ListAsync(spec);
    foreach (var video in videos)
    {
			if (string.IsNullOrEmpty(video.VideoId))
			{
				continue;
			}
      try
      {
        var response = await _getVideoService.ExecuteAsync(long.Parse(video.VideoId));
        if (response?.Data != null && response.Data.IsPlayable == false)
        {
					var deleteDequest = new DeleteVideoRequest(long.Parse(video.VideoId));
          await _deleteVideoService.ExecuteAsync(deleteDequest);
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
    var request = new GetVideosUserAppearsRequest();
    var videosResponse = await _getVideosUserAppearsService.ExecuteAsync(request);
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
					var deleteVideoRequest = new DeleteVideoRequest(video.Id);
          await _deleteVideoService.ExecuteAsync(deleteVideoRequest);
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
		  archiveVideo.NewVideoAdded();
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

	  var response = await _getVideoService.ExecuteAsync(videoId, cancellationToken);
	  if (response?.Data == null)
	  {
			return null;
		}

	  var existThumbsResponse = await _getAllAnimatedThumbnailService.ExecuteAsync(new GetAllSetAnimatedThumbnailsForVideoRequest(videoId, null), cancellationToken);
	  var animatedThumbnailUri = string.Empty;

		if (existThumbsResponse.Data.Total <= 0)
	  {
			var createSetAnimatedThumbnailsForVideoRequest = new CreateSetAnimatedThumbnailsForVideoRequest(videoId, 4);

			var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(createSetAnimatedThumbnailsForVideoRequest, cancellationToken);
		  if (getAnimatedThumbnailResult == null)
		  {
				return null;
			}
			animatedThumbnailUri = getAnimatedThumbnailResult.Data.AnimatedThumbsetUri;
	  }
	  else
	  {
		  animatedThumbnailUri = existThumbsResponse.Data.Data.FirstOrDefault()?.AnimatedThumbsetUri;
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
