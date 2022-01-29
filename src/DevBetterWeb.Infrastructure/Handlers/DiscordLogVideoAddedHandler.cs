using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogVideoAddedHandler : IHandle<VideoAddedEvent>
{
  private readonly BookDiscussionWebhook _webhook;

  public DiscordLogVideoAddedHandler(BookDiscussionWebhook webhook)
  {
    _webhook = webhook;
  }

  public static string returnWebhookMessageString(VideoAddedEvent domainEvent)
  {
    return $"Video {domainEvent.Video.Title} is added! " +
        $"Check out the video here: https://devbetter.com/Videos/Details/{domainEvent.Video.VideoId}.";
  }

  public Task Handle(VideoAddedEvent domainEvent)
  {
    _webhook.Content = returnWebhookMessageString(domainEvent);
    return _webhook.Send();
  }
}
