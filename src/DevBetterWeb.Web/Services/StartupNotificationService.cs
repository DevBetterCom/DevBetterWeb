using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Services;

public class StartupNotificationService : IHostedService
{
  //private static bool _notificationSent = false;
  private readonly IDomainEventDispatcher _dispatcher;
  private readonly ILogger<StartupNotificationService> _logger;

  public StartupNotificationService(IDomainEventDispatcher dispatcher,
      ILogger<StartupNotificationService> logger)
  {
    _dispatcher = dispatcher;
    _logger = logger;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("StartupNotificationService.StartAsync called");
    //if (!_notificationSent)
    //{
    await _dispatcher.Dispatch(new AppStartedEvent(DateTime.Now));
    //  _notificationSent = true;
    //}
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
