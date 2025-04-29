using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogMemberAddBookReadHandler : IHandle<MemberAddedBookReadEvent>
{
  private readonly BookDiscussionWebhook _webhook;

  public DiscordLogMemberAddBookReadHandler(BookDiscussionWebhook webhook)
  {
    _webhook = webhook;
  }

  public static string returnWebhookMessageString(MemberAddedBookReadEvent domainEvent)
  {
    return $"User {domainEvent.Member.FirstName} {domainEvent.Member.LastName} just finished reading {domainEvent.Book.Title}! " +
        $"Check out the leaderboard here: https://devbetter.com/Leaderboard.";
  }

  public Task Handle(MemberAddedBookReadEvent domainEvent, CancellationToken cancellationToken)
  {
    var message = returnWebhookMessageString(domainEvent);
    return _webhook.SendAsync(message);
  }
}
