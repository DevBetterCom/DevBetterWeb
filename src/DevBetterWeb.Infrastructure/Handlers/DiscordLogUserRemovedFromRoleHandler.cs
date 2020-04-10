using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogUserRemovedFromRoleHandler : IHandle<UserRemovedFromRoleEvent>
    {
        private readonly Webhook _webhook;

        public DiscordLogUserRemovedFromRoleHandler(Webhook webhook)
        {
            _webhook = webhook;
        }

        public async Task Handle(UserRemovedFromRoleEvent domainEvent)
        {
            _webhook.Content = $"User {domainEvent.EmailAddress} removed from role {domainEvent.Role}.";
            await _webhook.Send();
        }
    }
}
