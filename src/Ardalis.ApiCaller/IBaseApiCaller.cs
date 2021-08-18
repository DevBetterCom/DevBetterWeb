namespace Ardalis.ApiCaller
{
  public interface IBaseApiCaller<T>
  {
    public HttpResponse<T> Execute();
  }
}
