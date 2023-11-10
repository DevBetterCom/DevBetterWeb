using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogNewUserRegisteredHandler : IHandle<NewUserRegisteredEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogNewUserRegisteredHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(NewUserRegisteredEvent domainEvent)
  {
	  var message = $"New user registered with email address: {domainEvent.EmailAddress} from IP {domainEvent.IpAddress}.";
    return _webhook.SendAsync(message);
  }
}
