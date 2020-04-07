using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogForgotPasswordHandler : IHandle<PasswordResetEvent>
    {
        private readonly Webhook _webhook;

        public DiscordLogForgotPasswordHandler(Webhook webhook)
        {
            _webhook = webhook;
        }

        public async Task Handle(PasswordResetEvent domainEvent)
        {
            _webhook.Content = $"Password reset requested by {domainEvent.EmailAddress}.";
            await _webhook.Send();
        }
    }
}
