using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Services;
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
	private readonly ConcurrentQueue<Func<CancellationToken, ValueTask>> _workItems = new();
	private readonly SemaphoreSlim _signal = new(0);

	public void QueueBackgroundWorkItem(Func<CancellationToken, ValueTask> workItem)
	{
		if (workItem == null)
		{
			throw new ArgumentNullException(nameof(workItem));
		}
		_workItems.Enqueue(workItem);
		_signal.Release();
	}

	public async Task<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
	{
		await _signal.WaitAsync(cancellationToken);
		_workItems.TryDequeue(out var workItem);

		return workItem!;
	}
}
