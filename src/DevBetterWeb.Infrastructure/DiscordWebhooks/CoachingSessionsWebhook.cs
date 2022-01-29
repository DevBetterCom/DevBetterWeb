using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

public class CoachingSessionsWebhook : BaseWebhook
{
  public CoachingSessionsWebhook(IOptions<DiscordWebhookUrls> optionsAccessor,
    IAppLogger<BaseWebhook> logger) : base(optionsAccessor.Value.CoachingSessionsNotifications!, logger)
  {
    Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
    Guard.Against.NullOrEmpty(optionsAccessor.Value.CoachingSessionsNotifications, nameof(optionsAccessor.Value.CoachingSessionsNotifications));
  }

  public CoachingSessionsWebhook(string webhookUrl,
    IAppLogger<BaseWebhook> logger) : base(webhookUrl, logger)
  {
  }
}
