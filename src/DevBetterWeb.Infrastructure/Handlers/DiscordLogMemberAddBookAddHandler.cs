using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogMemberAddBookAddHandler : IHandle<MemberAddedBookAddEvent>
{
  private readonly BookDiscussionWebhook _webhook;

  public DiscordLogMemberAddBookAddHandler(BookDiscussionWebhook webhook)
  {
    _webhook = webhook;
  }

  public static string returnWebhookMessageString(MemberAddedBookAddEvent domainEvent)
  {
    return $"User {domainEvent.Member.FirstName} {domainEvent.Member.LastName} just added {domainEvent.Book.Title} to our book list!" +
        "Check out the leaderboard here: https://devbetter.com/Leaderboard.";
  }

  public Task Handle(MemberAddedBookAddEvent domainEvent, CancellationToken cancellationToken)
  {
    var message = returnWebhookMessageString(domainEvent);
    return _webhook.SendAsync(message);
  }
}
