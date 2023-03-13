using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class AppStartedEventHandler : IHandle<AppStartedEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public AppStartedEventHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }
  
  public async Task Handle(AppStartedEvent domainEvent)
  {
    var message = $"DevBetter.com web app started at {domainEvent.StartDateTime}.";
    await _webhook.SendAsync(message);
  }
}
