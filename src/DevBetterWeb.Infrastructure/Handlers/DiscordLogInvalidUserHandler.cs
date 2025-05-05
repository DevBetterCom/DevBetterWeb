using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogInvalidUserHandler : IHandle<InvalidUserEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogInvalidUserHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(InvalidUserEvent domainEvent, CancellationToken cancellationToken)
  {
    var message = $"Password reset requested by {domainEvent.EmailAddress} but no confirmed user found with that address.";
    return _webhook.SendAsync(message);
  }
}
