using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogNewBookAddedToGeneralListHandler : IHandle<NewBookCreatedEvent>
{
  private readonly BookDiscussionWebhook _webhook;

  public DiscordLogNewBookAddedToGeneralListHandler(BookDiscussionWebhook webhook)
  {
    _webhook = webhook;
  }

  public static string returnWebhookMessageString(NewBookCreatedEvent domainEvent)
  {
    return $"New book added to general book list: {domainEvent.Book.Title}! " +
        $"Check out the leaderboard here: https://devbetter.com/Leaderboard.";
  }

  public Task Handle(NewBookCreatedEvent domainEvent)
  {
    var message = returnWebhookMessageString(domainEvent);
    return _webhook.SendAsync(message);
  }
}
