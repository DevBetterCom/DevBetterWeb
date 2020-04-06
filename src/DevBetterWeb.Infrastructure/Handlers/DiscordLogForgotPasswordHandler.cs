using Ardalis.GuardClauses;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogForgotPasswordHandler : IHandle<PasswordResetEvent>
    {
        private readonly ILogger<DiscordLogForgotPasswordHandler> _logger;
        private readonly string _webhookUrl = "";

        public DiscordLogForgotPasswordHandler(ILogger<DiscordLogForgotPasswordHandler> logger,
            IOptions<DiscordWebhookUrls> optionsAccessor)
        {
            _logger = logger;
            Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));

            _webhookUrl = optionsAccessor.Value.AdminUpdates!;
        }

        public async Task Handle(PasswordResetEvent domainEvent)
        {
            _logger.LogWarning($"Handling password reset event - sending to discord URL: {_webhookUrl}.");

            var webhook = new Webhook(_webhookUrl);

            webhook.Content = $"Password reset requested by {domainEvent.EmailAddress}.";
            await webhook.Send();
        }
    }
}
