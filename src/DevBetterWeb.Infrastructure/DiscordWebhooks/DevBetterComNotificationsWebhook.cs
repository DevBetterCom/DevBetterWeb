using Ardalis.GuardClauses;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

public class DevBetterComNotificationsWebhook: BaseWebhook
{
	public DevBetterComNotificationsWebhook(IDiscordWebhookService discordWebhookService, IOptions<DiscordWebhookUrls> optionsAccessor) : base(discordWebhookService, optionsAccessor.Value.DevBetterComNotifications!)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
    Guard.Against.NullOrEmpty(optionsAccessor.Value.DevBetterComNotifications, nameof(optionsAccessor.Value.DevBetterComNotifications));
	}
}
