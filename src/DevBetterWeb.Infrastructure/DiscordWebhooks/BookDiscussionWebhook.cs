using Ardalis.GuardClauses;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

public class BookDiscussionWebhook: BaseWebhook
{

	public BookDiscussionWebhook(IDiscordWebhookService discordWebhookService, IOptions<DiscordWebhookUrls> optionsAccessor) : base(discordWebhookService, optionsAccessor.Value.BookDiscussion!)
	{
	  Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
    Guard.Against.NullOrEmpty(optionsAccessor.Value.BookDiscussion, nameof(optionsAccessor.Value.BookDiscussion));
	}
}
