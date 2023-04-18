using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace DevBetterWeb.Infrastructure.Services;
public class BackgroundTaskService : BackgroundService
{
	private readonly IBackgroundTaskQueue _taskQueue;

	public BackgroundTaskService(IBackgroundTaskQueue taskQueue)
	{
		_taskQueue = taskQueue;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var workItem = await _taskQueue.DequeueAsync(stoppingToken);

			try
			{
				if (workItem != null)
				{
					await workItem(stoppingToken);
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}
	}
}
