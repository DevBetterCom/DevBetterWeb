using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

public abstract class BaseWebhook
{
	private readonly IDiscordWebhookService _discordWebhookService;
  private readonly string _webHookUrl;

  public BaseWebhook(IDiscordWebhookService discordWebhookService, string webHookUrl)
  {
	  _discordWebhookService = discordWebhookService;
	  _webHookUrl = webHookUrl;
  }


  public virtual Task SendAsync(string message)
  {
	  return _discordWebhookService.SendAsync(_webHookUrl, message);
  }
}
