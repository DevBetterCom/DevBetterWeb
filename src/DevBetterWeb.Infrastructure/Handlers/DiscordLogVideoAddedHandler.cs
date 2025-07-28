using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using MediatR;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogVideoAddedHandler : IHandle<VideoAddedEvent>
{
  private readonly CoachingSessionsWebhook _webhook;

  public DiscordLogVideoAddedHandler(CoachingSessionsWebhook webhook)
  {
	  _webhook = webhook;
  }

  public static string ReturnWebhookMessageString(VideoAddedEvent domainEvent)
  {
    return $"Video {domainEvent.Video.Title} is added! " +
        $"Check out the video here: https://devbetter.com/Videos/Details/{domainEvent.Video.VideoId}.";
  }

  public Task Handle(VideoAddedEvent domainEvent, CancellationToken cancellationToken)
  {
		var message = ReturnWebhookMessageString(domainEvent);
    return _webhook.SendAsync(message);
  }
}
