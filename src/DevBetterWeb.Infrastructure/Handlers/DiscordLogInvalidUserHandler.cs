using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogInvalidUserHandler : IHandle<InvalidUserEvent>
    {
        private readonly Webhook _webhook;

        public DiscordLogInvalidUserHandler(Webhook webhook)
        {
            _webhook = webhook;
        }

        public async Task Handle(InvalidUserEvent domainEvent)
        {
            _webhook.Content = $"Password reset requested by {domainEvent.EmailAddress} but no confirmed user found with that address.";
            await _webhook.Send();
        }
    }
}
