using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogUserAddedToRoleHandler : IHandle<UserAddedToRoleEvent>
    {
        private readonly AdminUpdatesWebhook _webhook;

        public DiscordLogUserAddedToRoleHandler(AdminUpdatesWebhook webhook)
        {
            _webhook = webhook;
        }

        public async Task Handle(UserAddedToRoleEvent domainEvent)
        {
            _webhook.Content = $"User {domainEvent.EmailAddress} added to role {domainEvent.Role}.";
            await _webhook.Send();
        }
    }
}
