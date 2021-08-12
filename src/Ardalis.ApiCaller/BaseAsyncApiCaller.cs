using System.Threading;
using System.Threading.Tasks;

namespace Ardalis.ApiCaller
{
  /// <summary>
  /// A base class for an api caller that accepts parameters.
  /// </summary>
  /// <typeparam name="TRequest"></typeparam>
  /// <typeparam name="TResponse"></typeparam>
  public static class BaseAsyncApiCaller
  {
    public static class WithRequest<TRequest>
    {
      public abstract class WithResponse<TResponse>
      {
        public abstract Task<HttpResponse<TResponse>> ExecuteAsync(
          TRequest request,
          CancellationToken cancellationToken = default
        );
      }

      public abstract class WithoutResponse
      {
        public abstract Task<HttpResponse> ExecuteAsync(
          TRequest request,
          CancellationToken cancellationToken = default
        );
      }
    }

    public static class WithoutRequest
    {
      public abstract class WithResponse<TResponse>
      {
        public abstract Task<HttpResponse<TResponse>> ExecuteAsync(
          CancellationToken cancellationToken = default
        );
      }

      public abstract class WithoutResponse
      {
        public abstract Task<HttpResponse> ExecuteAsync(
          CancellationToken cancellationToken = default
        );
      }
    }
  }
}
