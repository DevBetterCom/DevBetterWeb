using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Infrastructure.Services;
public class BackgroundTaskService : BackgroundService
{
	private readonly IBackgroundTaskQueue _taskQueue;
	private readonly ILogger<BackgroundTaskService> _logger;

	public BackgroundTaskService(IBackgroundTaskQueue taskQueue, ILogger<BackgroundTaskService> logger)
	{
		_taskQueue = taskQueue;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var workItem = await _taskQueue.DequeueAsync(stoppingToken);

			try
			{
				await workItem(stoppingToken);
			}
			catch (IOException ioException)
			{
				_logger.LogError(ioException, "An I/O error occurred.");
			}
			catch (ArgumentNullException argumentNullException)
			{
				_logger.LogError(argumentNullException, "A null argument was passed.");
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "An unexpected error occurred.");
			}
		}
	}
}
