namespace DevBetterWeb.Client.Services;
public class BlazorWebService
{
  private readonly HttpClient _httpClient;

  public BlazorWebService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public HttpClient Client => _httpClient;
}
