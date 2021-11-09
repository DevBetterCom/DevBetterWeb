using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

public class DevBetterComNotificationsWebhook : BaseWebhook
{
  public DevBetterComNotificationsWebhook(IOptions<DiscordWebhookUrls> optionsAccessor, IAppLogger<BaseWebhook> logger) : base(optionsAccessor.Value.DevBetterComNotifications!, logger)
  {
    Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
    Guard.Against.NullOrEmpty(optionsAccessor.Value.DevBetterComNotifications, nameof(optionsAccessor.Value.DevBetterComNotifications));
  }

  public DevBetterComNotificationsWebhook(string webhookUrl, IAppLogger<BaseWebhook> logger) : base(webhookUrl, logger)
  {
  }
}
