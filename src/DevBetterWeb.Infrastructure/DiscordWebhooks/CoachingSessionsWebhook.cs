using Ardalis.GuardClauses;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

public class CoachingSessionsWebhook: BaseWebhook
{
	public CoachingSessionsWebhook(IDiscordWebhookService discordWebhookService, IOptions<DiscordWebhookUrls> optionsAccessor) : base(discordWebhookService, optionsAccessor.Value.CoachingSessionsNotifications!)
	{
    Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
    Guard.Against.NullOrEmpty(optionsAccessor.Value.CoachingSessionsNotifications, nameof(optionsAccessor.Value.CoachingSessionsNotifications));
	}
}
