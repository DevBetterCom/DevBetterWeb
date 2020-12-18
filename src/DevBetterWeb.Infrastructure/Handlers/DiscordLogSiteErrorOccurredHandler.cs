using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogSiteErrorOccurredHandler : IHandle<SiteErrorOccurredEvent>
    {
        private readonly AdminUpdatesWebhook _webhook;

        public DiscordLogSiteErrorOccurredHandler(AdminUpdatesWebhook webhook)
        {
            _webhook = webhook;
        }

        public async Task Handle(SiteErrorOccurredEvent domainEvent)
        {
            _webhook.Content = $"Site error: {domainEvent.SiteException.ToString()}.";
            await _webhook.Send();
        }
    }
}
