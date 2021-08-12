using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ardalis.ApiCaller
{
  public class HttpResponse<T>
  {
    public T Data { get; private set; }
    public string Text { get; }
    public HttpStatusCode Code { get; }
    public Dictionary<string, string[]> Headers { get; } = new Dictionary<string, string[]>();

    public HttpResponse(string result, HttpStatusCode code)
    {
      Text = result;
      Code = code;
      Headers = new Dictionary<string, string[]>();
    }

    public HttpResponse(T result, HttpStatusCode code)
    {
      Data = result;
      Code = code;
      Headers = new Dictionary<string, string[]>();
    }

    public HttpResponse(T result, HttpStatusCode code, HttpResponseHeaders headers) : this(result, code)
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
      var data = JsonSerializer.Deserialize<T>(textResult, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      Data = data;
      Code = result.StatusCode;
      Text = textResult;
      Headers = new Dictionary<string, string[]>();

      foreach (var header in result.Headers)
      {
        Headers.Add(header.Key, header.Value.ToArray());
      }
    }

    public void SetData(T data)
    {
      Data = data;
    }

    public static HttpResponse<T> FromHttpResponseMessage(HttpResponseMessage result)
    {
      return new HttpResponse<T>(result);
    }

    public static HttpResponse<T> FromHttpResponseMessage(T result, HttpStatusCode code)
    {
      return new HttpResponse<T>(result, code);
    }

    public static HttpResponse<T> FromHttpResponseMessage(T result, HttpStatusCode code, HttpResponseHeaders headers)
    {
      return new HttpResponse<T>(result, code, headers);
    }

    public static HttpResponse<T> FromException(string result)
    {
      return new HttpResponse<T>(result, HttpStatusCode.InternalServerError);
    }
  }

}
