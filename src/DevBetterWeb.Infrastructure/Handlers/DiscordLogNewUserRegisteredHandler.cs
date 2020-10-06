using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogNewUserRegisteredHandler : IHandle<NewUserRegisteredEvent>
    {
        private readonly Webhook _webhook;

        public DiscordLogNewUserRegisteredHandler(Webhook webhook)
        {
            _webhook = webhook;
        }

        public async Task Handle(NewUserRegisteredEvent domainEvent)
        {
            _webhook.Content = $"New user registered with email address: {domainEvent.EmailAddress} from IP {domainEvent.IpAddress}.";
            await _webhook.Send();
        }
    }
}
