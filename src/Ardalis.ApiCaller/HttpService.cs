using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ardalis.ApiCaller
{
  public class HttpService
  {
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public HttpService(HttpClient httpClient)
    {
      _httpClient = httpClient;
      _apiBaseUrl = _httpClient.BaseAddress.ToString();
    }

    public void SetTimeout(int minutes, TimeoutType timeType = TimeoutType.Seconds)
    {
      if(timeType == TimeoutType.Seconds)
      {
        _httpClient.Timeout = TimeSpan.FromSeconds(minutes);
      } 
      else if(timeType == TimeoutType.Minutes)
      {
        _httpClient.Timeout = TimeSpan.FromMinutes(minutes);
      }
      else if (timeType == TimeoutType.Hours)
      {
        _httpClient.Timeout = TimeSpan.FromHours(minutes);
      }   
    }

    public void SetAuthorization(string value)
    {
      _httpClient.DefaultRequestHeaders.Remove("Authorization");
      _httpClient.DefaultRequestHeaders.Add("Authorization", value);
    }

    public void SetDefaultTimeout()
    {
      _httpClient.Timeout = TimeSpan.FromSeconds(60);
    }

    
    public async Task<HttpResponse<T>> HttpGetAsync<T>(string uri)
        where T : class
    {
      var result = await _httpClient.GetAsync($"{_apiBaseUrl}{uri}");

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public Task<HttpResponse<T>> HttpDeleteAsync<T>(string uri, object id)
        where T : class
    {
      return HttpDeleteAsync<T>($"{uri}/{id}");
    }

    public async Task<HttpResponse<T>> HttpDeleteAsync<T>(string uri)
        where T : class
    {
      var result = await _httpClient.DeleteAsync($"{_apiBaseUrl}{uri}");

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<bool>> HttpDeleteAsync(string uri)
    {
      var result = await _httpClient.DeleteAsync($"{_apiBaseUrl}{uri}");

      return HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode, result.Headers);
    }

    public async Task<HttpResponse<T>> HttpPostAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PostAsync($"{_apiBaseUrl}{uri}", content);

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPostByQueryAsync<T>(string uri, QueryBuilder query)
      where T : class
    {
      var result = await _httpClient.PostAsync($"{_apiBaseUrl}{uri}?{query.Build()}", null);
      Thread.Sleep(1000);
      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPostByFormAsync<T>(string uri, QueryBuilder query)
      where T : class
    {
      var formContent = new FormUrlEncodedContent(query.ToListKeyValuePair().ToArray());
      var result = await _httpClient.PostAsync($"{_apiBaseUrl}{uri}", formContent);

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPostByStringAsync<T>(string uri, string body)
      where T : class
    {

      var result = await _httpClient.PostAsync($"{_apiBaseUrl}{uri}", new StringContent(body));

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPutJsonAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PutAsync($"{_apiBaseUrl}{uri}", content);

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }
    
    public async Task<HttpResponse<T>> HttpPatchAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PatchAsync($"{_apiBaseUrl}{uri}", content);

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }
    
    public async Task<HttpResponse<bool>> HttpPatchWithoutResponseAsync(string uri, object dataToSend)
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PatchAsync($"{_apiBaseUrl}{uri}", content);

      return HttpResponse<bool>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPutBytesAsync<T>(string uri, byte[] dataToSend)
      where T : class
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await _httpClient.PutAsync($"{_apiBaseUrl}{uri}", content);

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<bool>> HttpPutBytesAsync(string uri, byte[] dataToSend)
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await _httpClient.PutAsync($"{_apiBaseUrl}{uri}", content);

      return HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode);
    }

    public async Task<bool> HttpPutBytesWithoutResponseAsync(string uri, byte[] dataToSend)
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await _httpClient.PutAsync($"{_apiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return false;
      }

      return true;
    }

    private StringContent ToJson(object obj)
    {
      return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
    }
  }
}
