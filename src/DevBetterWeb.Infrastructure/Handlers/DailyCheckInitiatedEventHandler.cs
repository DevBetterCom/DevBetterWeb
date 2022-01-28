﻿using System;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.Core.Handlers;

public class DailyCheckInitiatedEventHandler : IHandle<DailyCheckInitiatedEvent>
{
  private const string DAILY_CHECK_COMPLETED_MESSAGE = "Daily Check Event Completed";

  private readonly AdminUpdatesWebhook _webhook;
  private readonly IAlumniGraduationService _alumniGraduationService;
  private readonly IDailyCheckPingService _dailyCheckPingService;
  private readonly IDailyCheckSubscriptionPlanCountService _dailyCheckSubscriptionPlanCountService;
  private readonly IRepository<DailyCheck> _repository;
  private readonly IRepository<ArchiveVideo> _repositoryArchiveVideo;
  private readonly CreateAnimatedThumbnailsService _createAnimatedThumbnailsService;

  public DailyCheckInitiatedEventHandler(AdminUpdatesWebhook webhook,
    IAlumniGraduationService alumniGraduationService,
    IDailyCheckPingService dailyCheckPingService,
    IDailyCheckSubscriptionPlanCountService dailyCheckSubscriptionPlanCountService,
    IRepository<DailyCheck> repository,
    IRepository<ArchiveVideo> repositoryArchiveVideo,
    CreateAnimatedThumbnailsService createAnimatedThumbnailsService)
  {
    _webhook = webhook;
    _alumniGraduationService = alumniGraduationService;
    _dailyCheckPingService = dailyCheckPingService;
    _dailyCheckSubscriptionPlanCountService = dailyCheckSubscriptionPlanCountService;
    _repository = repository;
    _repositoryArchiveVideo = repositoryArchiveVideo;
    _createAnimatedThumbnailsService = createAnimatedThumbnailsService;
  }

  public async Task Handle(DailyCheckInitiatedEvent domainEvent)
  {
    AppendOnlyStringList messages = new();

    await _dailyCheckPingService.PingAdminsAboutAlmostAlumsIfNeeded(messages);

    await _alumniGraduationService.GraduateMembersIfNeeded(messages);

    await _dailyCheckPingService.DeactiveInvitesForExistingMembers(messages);

    await _dailyCheckPingService.SendPingIfNeeded(messages);

    // check if number of MemberSubscriptionPlans == expected number
    await _dailyCheckSubscriptionPlanCountService.WarnIfNumberOfMemberSubscriptionPlansDifferentThanExpected(messages);

    await UpdateVideosThumbnail();

    messages.Append(DAILY_CHECK_COMPLETED_MESSAGE);

    await SendMessagesToDiscord(messages);
    await StoreMessagesInTasksCompleted(messages);
  }

  private async Task UpdateVideosThumbnail()
  {
    var spec = new ArchiveVideoWithoutThumbnailSpec();
    var videos = await _repositoryArchiveVideo.ListAsync(spec);
    foreach (var video in videos)
    {
      if (video?.VideoId == null)
      {
        continue;
      }
      var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(long.Parse(video.VideoId));

      video.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
      await _repositoryArchiveVideo.UpdateAsync(video);
    }
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
        if (!message.Equals(DAILY_CHECK_COMPLETED_MESSAGE))
        {
          if (todaysDailyCheck.TasksCompleted == null) todaysDailyCheck.TasksCompleted = "";
          todaysDailyCheck.TasksCompleted += message;
        }
      }
      await _repository.UpdateAsync(todaysDailyCheck);
    }
  }
}
