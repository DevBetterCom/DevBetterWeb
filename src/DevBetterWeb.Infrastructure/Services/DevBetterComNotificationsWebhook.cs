using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.Services
{
  public class DevBetterComNotificationsWebhook : BaseWebhook
  {
    public DevBetterComNotificationsWebhook(IOptions<DiscordWebhookUrls> optionsAccessor) : base(optionsAccessor.Value.DevBetterComNotifications!)
    {
      Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
      Guard.Against.NullOrEmpty(optionsAccessor.Value.DevBetterComNotifications, nameof(optionsAccessor.Value.DevBetterComNotifications));
    }

    public DevBetterComNotificationsWebhook(string webhookUrl) : base(webhookUrl)
    {
    }
  }
}
