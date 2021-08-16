using System;
using System.Net.Http;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Tests.Constants;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class HttpServiceBuilder
  {
    public static HttpService Build()
    {
      var vimeoToken = Environment.GetEnvironmentVariable("VIMEO_TOKEN");
      var httpClient = new HttpClient { BaseAddress = new Uri(vimeoToken) };
      httpClient.DefaultRequestHeaders.Remove("Accept");
      httpClient.DefaultRequestHeaders.Add("Accept", ServiceConstants.VIMEO_HTTP_ACCEPT);
      httpClient.Timeout = TimeSpan.FromMinutes(60);
      var httpService = new HttpService(httpClient);
      httpService.SetAuthorization(AccountConstants.ACCESS_TOKEN);

      return httpService;
    }
  }
}
