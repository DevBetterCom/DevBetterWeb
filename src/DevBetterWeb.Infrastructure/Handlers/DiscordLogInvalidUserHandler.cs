using Ardalis.GuardClauses;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Handlers;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogInvalidUserHandler : IHandle<InvalidUserEvent>
    {
        private readonly string _webhookUrl = "";

        public DiscordLogInvalidUserHandler(IOptions<DiscordWebhookUrls> optionsAccessor)
        {
            Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
            Guard.Against.NullOrEmpty(optionsAccessor.Value.AdminUpdates, "AdminUpdates");

            _webhookUrl = optionsAccessor.Value.AdminUpdates!;
        }

        public async Task Handle(InvalidUserEvent domainEvent)
        {
            var webhook = new Webhook(_webhookUrl);

            webhook.Content = $"Password reset requested by {domainEvent.EmailAddress} but no confirmed user found with that address.";
            await webhook.Send();
        }
    }
}
