using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using System;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
  public class DailyCheckInitiatedEventHandler : IHandle<DailyCheckInitiatedEvent>
  {
    private readonly AdminUpdatesWebhook _webhook;
    private readonly IAlumniGraduationService _alumniGraduationService;
    private readonly IDailyCheckPingService _dailyCheckPingService;
    private readonly IRepository<DailyCheck> _repository;

    public DailyCheckInitiatedEventHandler(AdminUpdatesWebhook webhook,
      IAlumniGraduationService alumniGraduationService,
      IDailyCheckPingService dailyCheckPingService,
      IRepository<DailyCheck> repository)
    {
      _webhook = webhook;
      _alumniGraduationService = alumniGraduationService;
      _dailyCheckPingService = dailyCheckPingService;
      _repository = repository;
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

      await SendMessagesToDiscord(messages);
      await StoreMessagesInTasksCompleted(messages);
    }

    private async Task SendMessagesToDiscord(AppendOnlyStringList messages)
    {
      foreach (var message in messages)
      {
        _webhook.Content = message;
        await _webhook.Send();
      }
    }

    private async Task StoreMessagesInTasksCompleted(AppendOnlyStringList messages)
    {
      var spec = new DailyCheckByDateSpec(DateTime.Today);
      var todaysDailyCheck = await _repository.GetBySpecAsync(spec);

      if (todaysDailyCheck != null)
      {
        foreach (var message in messages)
        {
          if (!message.Equals("Daily Check Event Completed"))
          {
            if (todaysDailyCheck.TasksCompleted == null) todaysDailyCheck.TasksCompleted = "";
            todaysDailyCheck.TasksCompleted += message;
          }
        }
        await _repository.UpdateAsync(todaysDailyCheck);
      }
    }
  }
}
