using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogUserRemovedFromRoleHandler : IHandle<UserRemovedFromRoleEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogUserRemovedFromRoleHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(UserRemovedFromRoleEvent domainEvent)
  {
    var message = $"User {domainEvent.EmailAddress} removed from role {domainEvent.Role}.";
    return _webhook.SendAsync(message);
  }
}
