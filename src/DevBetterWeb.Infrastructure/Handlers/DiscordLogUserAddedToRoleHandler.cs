using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers
{
  public class DiscordLogUserAddedToRoleHandler : IHandle<UserAddedToRoleEvent>
  {
    private readonly AdminUpdatesWebhook _webhook;

    public DiscordLogUserAddedToRoleHandler(AdminUpdatesWebhook webhook)
    {
      _webhook = webhook;
    }

    public Task Handle(UserAddedToRoleEvent domainEvent)
    {
      _webhook.Content = $"User {domainEvent.EmailAddress} added to role {domainEvent.Role}.";
      return _webhook.Send();
    }
  }
}
