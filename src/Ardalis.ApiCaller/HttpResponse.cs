using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Ardalis.ApiCaller
{
  public class HttpResponse
  {
    public string Text { get; }
    public HttpStatusCode Code { get; }
    public Dictionary<string, string[]> Headers { get; } = new Dictionary<string, string[]>();

    public HttpResponse(HttpStatusCode code)
    {
      Code = code;
    }

    public HttpResponse(string result, HttpStatusCode code)
    {
      Text = result;
      Code = code;
    }

    public HttpResponse(HttpStatusCode code, HttpResponseHeaders headers) : this(code)
    {
      foreach (var header in headers)
      {
        Headers.Add(header.Key, header.Value.ToArray());
      }
    }

    public string GetFirstHeader(string key)
    {
      Headers.TryGetValue(key, out var allValues);

      return allValues?.FirstOrDefault();
    }

    public string[] GetHeader(string key)
    {
      Headers.TryGetValue(key, out var allValues);

      return allValues;
    }

    public HttpResponse(HttpResponseMessage result)
    {
      var textResult = result.Content.ReadAsStringAsync().Result;

      Code = result.StatusCode;
      Text = textResult;
      Headers = new Dictionary<string, string[]>();

      foreach (var header in result.Headers)
      {
        Headers.Add(header.Key, header.Value.ToArray());
      }
    }

    public static HttpResponse FromHttpResponseMessage(HttpResponseMessage result)
    {
      return new HttpResponse(result);
    }

    public static HttpResponse FromHttpResponseMessage(HttpStatusCode code)
    {
      return new HttpResponse(code);
    }

    public static HttpResponse FromHttpResponseMessage(HttpStatusCode code, HttpResponseHeaders headers)
    {
      return new HttpResponse(code, headers);
    }

    public static HttpResponse FromException(string result)
    {
      return new HttpResponse(result, HttpStatusCode.InternalServerError);
    }
  }
}
