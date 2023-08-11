using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace DevBetterWeb.FunctionalTests;

public class NoOpMediator : IMediator
{
	public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	public Task Publish(object notification, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
	{
		return Task.CompletedTask;
	}

	public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult<TResponse>(default);
	}

	public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
	{
		return Task.CompletedTask;
	}

	public Task<object> Send(object request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult<object>(default);
	}
}
