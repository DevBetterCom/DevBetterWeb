﻿using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogSiteErrorOccurredHandler : IHandle<SiteErrorOccurredEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogSiteErrorOccurredHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(SiteErrorOccurredEvent domainEvent, CancellationToken cancellationToken)
  {
	  var message = $"Site error: {domainEvent.SiteException.ToString()}.";
    return _webhook.SendAsync(message);
  }
}
