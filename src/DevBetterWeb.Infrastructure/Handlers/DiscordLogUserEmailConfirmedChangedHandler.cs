using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Infrastructure.Handlers;

public class DiscordLogUserEmailConfirmedChangedHandler : IHandle<UserEmailConfirmedChangedEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogUserEmailConfirmedChangedHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(UserEmailConfirmedChangedEvent domainEvent)
  {
    var message = $"For the user {domainEvent.EmailAddress} EmailConfirmed value has been changed to: {domainEvent.IsEmailConfirmed}";
    return _webhook.SendAsync(message);
  }
}
