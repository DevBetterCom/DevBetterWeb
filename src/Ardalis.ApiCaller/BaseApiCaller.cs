namespace Ardalis.ApiCaller
{
  /// <summary>
  /// A base class for an api caller that accepts parameters.
  /// </summary>
  /// <typeparam name="TRequest"></typeparam>
  /// <typeparam name="TResponse"></typeparam>
  public static class BaseApiCaller
  {
    public static class WithRequest<TRequest>
    {
      public abstract class WithResponse<TResponse>
      {
        public abstract HttpResponse<TResponse> Execute(TRequest request);
      }

      public abstract class WithoutResponse
      {
        public abstract HttpResponse Execute(TRequest request);
      }
    }

    public static class WithoutRequest
    {
      public abstract class WithResponse<TResponse>
      {
        public abstract HttpResponse<TResponse> Execute();
      }

      public abstract class WithoutResponse
      {
        public abstract HttpResponse Execute();
      }
    }
  }
}
