using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Services
{
  public class DailyCheck : BackgroundService
  {
    private const int DELAY_IN_MILLISECONDS = 3600000;

    private readonly ILogger<DailyCheck> _logger;
    private readonly IDomainEventDispatcher _dispatcher;

    public DailyCheck(ILogger<DailyCheck> logger,
      IDomainEventDispatcher dispatcher)
    {
      _logger = logger;
      _dispatcher = dispatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        if (!DailyCheckRanToday())
        {
          _logger.LogInformation("Daily Check running at: {time}", DateTimeOffset.Now);

          RaiseDailyCheckEvent();

          await Task.Delay(DELAY_IN_MILLISECONDS, stoppingToken);
        }
      }
    }

    private bool DailyCheckRanToday()
    {
      return false;
    }

    private async void RaiseDailyCheckEvent()
    {
      await _dispatcher.Dispatch(new DailyCheckEvent());
      _logger.LogInformation("Daily Check Event Raised");
    }
  }
}
