using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogExceptionHandler : IHandle<ExceptionEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogExceptionHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(ExceptionEvent domainEvent, CancellationToken cancellationToken)
  {
    var message = $"Exception {DateTime.UtcNow}: {domainEvent.Exception}";
    return _webhook.SendAsync(message);
  }
}
