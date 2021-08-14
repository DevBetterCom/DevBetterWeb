using System;
using System.Net.Http;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class HttpServiceBuilder
  {
    public static HttpService Build()
    {
      var httpClient = new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) };
      httpClient.DefaultRequestHeaders.Remove("Accept");
      httpClient.DefaultRequestHeaders.Add("Accept", ServiceConstants.VIMEO_HTTP_ACCEPT);
      httpClient.Timeout = TimeSpan.FromMinutes(60);
      var httpService = new HttpService(httpClient);

      return httpService;
    }
  }
}
