using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Core.Handlers;

public class UpdateThumbVideoAddedHandler : IHandle<VideoAddedEvent>
{
	private readonly IVideosService _videosService;

  public UpdateThumbVideoAddedHandler(IVideosService videosService)
  {
	  _videosService = videosService;
  }

  public Task Handle(VideoAddedEvent domainEvent)
  {
	  long.TryParse(domainEvent.Video.VideoId, out var videoId);
		
		return _videosService.UpdateVideoThumbnailsAsync(videoId);
  }
}
