using Ardalis.GuardClauses;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

public class AdminUpdatesWebhook: BaseWebhook
{
	public AdminUpdatesWebhook(IDiscordWebhookService discordWebhookService, 
		IOptions<DiscordWebhookUrls> optionsAccessor): base(discordWebhookService, optionsAccessor.Value.AdminUpdates!)
	{
		Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
		Guard.Against.NullOrEmpty(optionsAccessor.Value.AdminUpdates, nameof(optionsAccessor.Value.AdminUpdates));
	}
}
