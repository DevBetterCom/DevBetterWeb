using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.Services
{
  public class AdminUpdatesWebhook : BaseWebhook
  {
    public AdminUpdatesWebhook(IOptions<DiscordWebhookUrls> optionsAccessor) : base(optionsAccessor.Value.AdminUpdates!)
    {
      Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
      Guard.Against.NullOrEmpty(optionsAccessor.Value.AdminUpdates, nameof(optionsAccessor.Value.AdminUpdates));
    }

    public AdminUpdatesWebhook(string webhookUrl) : base(webhookUrl)
    {
    }
  }
}
