using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
  public class DailyCheckInitiatedEventHandler : IHandle<DailyCheckInitiatedEvent>
  {
    private readonly AdminUpdatesWebhook _webhook;
    private readonly IAlumniGraduationService _alumniGraduationService;

    public DailyCheckInitiatedEventHandler(AdminUpdatesWebhook webhook,
      IAlumniGraduationService alumniGraduationService)
    {
      _webhook = webhook;
      _alumniGraduationService = alumniGraduationService;
    }

    public async Task Handle(DailyCheckInitiatedEvent domainEvent)
    {
      AppendOnlyStringList messages = new();
      
      // Add real stuff

      // check if people need upgraded to alumni

      // check if people need to be pinged about new member link

      _webhook.Content = "Daily Check Event Completed";
      await _webhook.Send();
    }
  }
}
