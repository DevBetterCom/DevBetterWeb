using System;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.Infrastructure.Services;

public class VideosThumbnailService : IVideosThumbnailService
{
  private readonly IRepository<ArchiveVideo> _repositoryArchiveVideo;
  private readonly CreateAnimatedThumbnailsService _createAnimatedThumbnailsService;

  public VideosThumbnailService(IRepository<ArchiveVideo> repositoryArchiveVideo, CreateAnimatedThumbnailsService createAnimatedThumbnailsService)
  {
    _repositoryArchiveVideo = repositoryArchiveVideo;
    _createAnimatedThumbnailsService = createAnimatedThumbnailsService;
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
        var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(long.Parse(video.VideoId));
        if (getAnimatedThumbnailResult == null)
        {
          continue;
        }
        video.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
        await _repositoryArchiveVideo.UpdateAsync(video);
        messages.Append($"Video {video.VideoId} updated with Thumbnails.");
      }
      catch (Exception)
      {
        // ignored
      }
    }
  }
}
