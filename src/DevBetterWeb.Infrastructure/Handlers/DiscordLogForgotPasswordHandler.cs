﻿using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogForgotPasswordHandler : IHandle<PasswordResetEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogForgotPasswordHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(PasswordResetEvent domainEvent, CancellationToken cancellationToken)
  {
    var message = $"Password reset requested by {domainEvent.EmailAddress}.";
    return _webhook.SendAsync(message);
  }
}
