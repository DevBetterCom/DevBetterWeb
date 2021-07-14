using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.DiscordWebooks
{
  public class AdminUpdatesWebhook : BaseWebhook
  {
    public AdminUpdatesWebhook(IOptions<DiscordWebhookUrls> optionsAccessor,
      IAppLogger<BaseWebhook> logger) : base(optionsAccessor.Value.AdminUpdates!, logger)
    {
      Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
      Guard.Against.NullOrEmpty(optionsAccessor.Value.AdminUpdates, nameof(optionsAccessor.Value.AdminUpdates));
    }

    public AdminUpdatesWebhook(string webhookUrl, IAppLogger<BaseWebhook> logger) : base(webhookUrl, logger)
    {
    }
  }
}
