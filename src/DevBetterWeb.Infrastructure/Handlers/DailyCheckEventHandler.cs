using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
  public class DailyCheckEventHandler : IHandle<DailyCheckEvent>
  {
    private readonly AdminUpdatesWebhook _webhook;

    public DailyCheckEventHandler(AdminUpdatesWebhook webhook)
    {
      _webhook = webhook;
    }

    public async Task Handle(DailyCheckEvent domainEvent)
    {
      
      // Add real stuff

      _webhook.Content = "Daily Check Event Completed";
      await _webhook.Send();
    }
  }
}
