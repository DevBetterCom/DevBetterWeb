using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks
{
  public class BookDiscussionWebhook : BaseWebhook
  {
    public BookDiscussionWebhook(IOptions<DiscordWebhookUrls> optionsAccessor,
      IAppLogger<BaseWebhook> logger) : base(optionsAccessor.Value.BookDiscussion!, logger)
    {
      Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
      Guard.Against.NullOrEmpty(optionsAccessor.Value.BookDiscussion, nameof(optionsAccessor.Value.BookDiscussion));
    }

    public BookDiscussionWebhook(string webhookUrl,
      IAppLogger<BaseWebhook> logger) : base(webhookUrl, logger)
    {
    }
  }
}
