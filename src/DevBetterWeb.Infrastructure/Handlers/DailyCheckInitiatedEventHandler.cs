using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
  public class DailyCheckInitiatedEventHandler : IHandle<DailyCheckInitiatedEvent>
  {
    private readonly AdminUpdatesWebhook _webhook;

    public DailyCheckInitiatedEventHandler(AdminUpdatesWebhook webhook)
    {
      _webhook = webhook;
    }

    public async Task Handle(DailyCheckInitiatedEvent domainEvent)
    {
      
      // Add real stuff

      // check if people need upgraded to alumni

      // check if people need to be pinged about new member link

      _webhook.Content = "Daily Check Event Completed";
      await _webhook.Send();
    }
  }
}
