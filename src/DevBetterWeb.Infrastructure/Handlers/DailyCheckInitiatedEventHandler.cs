using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
  public class DailyCheckInitiatedEventHandler : IHandle<DailyCheckInitiatedEvent>
  {
    private readonly AdminUpdatesWebhook _webhook;
    private readonly IAlumniGraduationService _alumniGraduationService;
    private readonly IDailyCheckPingService _dailyCheckPingService;

    public DailyCheckInitiatedEventHandler(AdminUpdatesWebhook webhook,
      IAlumniGraduationService alumniGraduationService,
      IDailyCheckPingService dailyCheckPingService)
    {
      _webhook = webhook;
      _alumniGraduationService = alumniGraduationService;
      _dailyCheckPingService = dailyCheckPingService;
    }

    public async Task Handle(DailyCheckInitiatedEvent domainEvent)
    {
      AppendOnlyStringList messages = new();

      // real stuff

      // check if admins need to be reminded to avoid renewing near-alumnus's subscription

      // check if people need upgraded to alumni
      await _alumniGraduationService.GraduateMembersIfNeeded(messages);

      // check if people need to be pinged about new member link
      await _dailyCheckPingService.SendPingIfNeeded(messages);

      messages.Append("Daily Check Event Completed");

      foreach (var message in messages)
      {
        _webhook.Content = message;
        await _webhook.Send();
      }
    }
  }
}
