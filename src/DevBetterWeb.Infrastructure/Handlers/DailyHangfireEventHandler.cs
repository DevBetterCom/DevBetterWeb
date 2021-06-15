using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;

namespace DevBetterWeb.Core.Handlers
{
  public class DailyHangfireEventHandler : IHandle<DailyHangfireEvent>
  {
    private readonly AdminUpdatesWebhook _webhook;

    public DailyHangfireEventHandler(AdminUpdatesWebhook webhook)
    {
      _webhook = webhook;
    }

    public Task Handle(DailyHangfireEvent hangfireEvent)
    {
      _webhook.Content = "Hangfire ran!";
      return _webhook.Send();
    }
  }
}
