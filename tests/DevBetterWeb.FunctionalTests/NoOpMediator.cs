#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;

namespace DevBetterWeb.FunctionalTests;

public class NoOpMediator : IMediator
{
	public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamQuery<TResponse> query, CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamCommand<TResponse> command, CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	public ValueTask Publish(object notification, CancellationToken cancellationToken = default)
	{
		return ValueTask.CompletedTask;
	}

	public ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
	{
		return ValueTask.CompletedTask;
	}

	public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		return ValueTask.FromResult<TResponse>(default!);
	}

	public ValueTask<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
	{
		return ValueTask.FromResult<TResponse>(default!);
	}

	public ValueTask<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
	{
		return ValueTask.FromResult<TResponse>(default!);
	}

	public ValueTask<object?> Send(object request, CancellationToken cancellationToken = default)
	{
		return ValueTask.FromResult<object?>(default);
	}
}
