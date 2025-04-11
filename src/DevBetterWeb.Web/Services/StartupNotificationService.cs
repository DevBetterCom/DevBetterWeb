using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Services;

public class StartupNotificationService : IHostedService
{
	private readonly IMediator _mediator;
	private readonly ILogger<StartupNotificationService> _logger;

  public StartupNotificationService(IMediator mediator,
      ILogger<StartupNotificationService> logger)
  {
		_mediator = mediator;
		_logger = logger;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("StartupNotificationService.StartAsync called");
    //if (!_notificationSent)
    //{
    await _mediator.Publish(new AppStartedEvent(DateTime.Now));
    //  _notificationSent = true;
    //}
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
