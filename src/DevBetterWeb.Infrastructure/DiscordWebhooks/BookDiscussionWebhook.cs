using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks
{
  public class BookDiscussionWebhook : BaseWebhook
  {
    public BookDiscussionWebhook(IOptions<DiscordWebhookUrls> optionsAccessor) : base(optionsAccessor.Value.BookDiscussion!)
    {
      Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
      Guard.Against.NullOrEmpty(optionsAccessor.Value.BookDiscussion, nameof(optionsAccessor.Value.BookDiscussion));
    }

    public BookDiscussionWebhook(string webhookUrl) : base(webhookUrl)
    {
    }
  }
}
