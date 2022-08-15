using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogVideoAddedHandler : IHandle<VideoAddedEvent>
{
  private readonly CoachingSessionsWebhook _webhook;
  private readonly IVideosCacheService _videosCacheService;

  public DiscordLogVideoAddedHandler(CoachingSessionsWebhook webhook, IVideosCacheService videosCacheService)
  {
	  _webhook = webhook;
	  _videosCacheService = videosCacheService;
  }

  public static string ReturnWebhookMessageString(VideoAddedEvent domainEvent)
  {
    return $"Video {domainEvent.Video.Title} is added! " +
        $"Check out the video here: https://devbetter.com/Videos/Details/{domainEvent.Video.VideoId}.";
  }

  public Task Handle(VideoAddedEvent domainEvent)
  {
	  _videosCacheService.UpdateAllVideosAsync();

		_webhook.Content = ReturnWebhookMessageString(domainEvent);
    return _webhook.Send();
  }
}
