using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;
public interface IBackgroundTaskQueue
{
	void QueueBackgroundWorkItem(Func<CancellationToken, ValueTask> workItem);
	Task<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
