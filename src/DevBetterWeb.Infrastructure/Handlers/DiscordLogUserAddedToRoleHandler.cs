using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogUserAddedToRoleHandler : IHandle<UserAddedToRoleEvent>
    {
        private readonly Webhook _webhook;

        public DiscordLogUserAddedToRoleHandler(Webhook webhook)
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
