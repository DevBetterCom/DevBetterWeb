using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ardalis.ApiCaller
{
  public class HttpService
  {
    private HttpClient _httpClient;
    private string ApiBaseUrl => _httpClient.BaseAddress == null ? string.Empty: _httpClient.BaseAddress.ToString();

    public HttpService(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public void ResetHttp(string baseUri=null, string accept=null)
    {
      Uri baseUriToAdd = null;

      if (baseUri == null)
      {
        baseUriToAdd = _httpClient.BaseAddress;
      }
      else
      {
        baseUriToAdd = new Uri(baseUri);
      }
      
      var acceptToAdd = accept;
      if (string.IsNullOrEmpty(accept))
      {
        acceptToAdd = _httpClient.DefaultRequestHeaders.Accept.First()?.ToString();
      }
      var token = GetFirstHeader("Authorization");
      var timeout = _httpClient.Timeout;

      _httpClient = new HttpClient();
      _httpClient.BaseAddress = baseUriToAdd;
      _httpClient.DefaultRequestHeaders.Add("accept", acceptToAdd);
      _httpClient.DefaultRequestHeaders.Add("Authorization", token);
      _httpClient.Timeout = timeout;
    }

    public void ResetBaseUri()
    {
      var acceptToAdd = _httpClient.DefaultRequestHeaders.Accept.First()?.ToString();
      
      // TODO: Eliminate duplication with above method
      var token = GetFirstHeader("Authorization");
      var timeout = _httpClient.Timeout;

      _httpClient = new HttpClient();
      _httpClient.DefaultRequestHeaders.Add("accept", acceptToAdd);
      _httpClient.DefaultRequestHeaders.Add("Authorization", token);
      _httpClient.Timeout = timeout;
    }

    public string GetFirstHeader(string key)
    {
      var allValues = _httpClient.DefaultRequestHeaders.GetValues(key);

      return allValues.FirstOrDefault();
    }

    public string[] GetHeader(string key)
    {
      var allValues = _httpClient.DefaultRequestHeaders.GetValues(key);

      return allValues.ToArray();
    }

    public void SetTimeout(int units, TimeoutType timeType = TimeoutType.Seconds)
    {
      if(timeType == TimeoutType.Seconds)
      {
        _httpClient.Timeout = TimeSpan.FromSeconds(units);
      } 
      else if(timeType == TimeoutType.Minutes)
      {
        _httpClient.Timeout = TimeSpan.FromMinutes(units);
      }
      else if (timeType == TimeoutType.Hours)
      {
        _httpClient.Timeout = TimeSpan.FromHours(units);
      }   
    }

    public void SetAuthorization(string value)
    {
      _httpClient.DefaultRequestHeaders.Remove("Authorization");
      _httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {value}");
    }

    public void SetDefaultTimeout()
    {
      _httpClient.Timeout = TimeSpan.FromSeconds(60);
    }
    
    public async Task<HttpResponse<T>> HttpGetAsync<T>(string uri, QueryBuilder query=null)
        where T : class
    {
      var uriToSend = query == null || query.IsEmpty()? $"{ApiBaseUrl}{uri}" : $"{ApiBaseUrl}{uri}?{query.Build()}";
      var result = await _httpClient.GetAsync(uriToSend);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

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
      var result = await _httpClient.DeleteAsync($"{ApiBaseUrl}{uri}");
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<bool>> HttpDeleteAsync(string uri)
    {
      var result = await _httpClient.DeleteAsync($"{ApiBaseUrl}{uri}");
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode, result.Headers);
    }

    public async Task<HttpResponse<T>> HttpPostAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PostAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPostByQueryAsync<T>(string uri, QueryBuilder query)
      where T : class
    {
      var result = await _httpClient.PostAsync($"{ApiBaseUrl}{uri}?{query.Build()}", null);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPostByFormAsync<T>(string uri, QueryBuilder query)
      where T : class
    {
      var formContent = new FormUrlEncodedContent(query.ToListKeyValuePair().ToArray());
      var result = await _httpClient.PostAsync($"{ApiBaseUrl}{uri}", formContent);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPostByStringAsync<T>(string uri, string body)
      where T : class
    {

      var result = await _httpClient.PostAsync($"{ApiBaseUrl}{uri}", new StringContent(body));
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<T>> HttpPutJsonAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }
    
    public async Task<HttpResponse<T>> HttpPatchAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }
    
    public async Task<HttpResponse<bool>> HttpPatchWithoutResponseAsync(string uri, object dataToSend)
    {
      var content = ToJson(dataToSend);

      var result = await _httpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(false, result.StatusCode);
      }

      return HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode);
    }

    public async Task<HttpResponse<T>> HttpPutBytesAsync<T>(string uri, byte[] dataToSend)
      where T : class
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await _httpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<T>.FromHttpResponseMessage(result);
    }

    public async Task<HttpResponse<bool>> HttpPutBytesAsync(string uri, byte[] dataToSend)
    {
      ByteArrayContent content = null;
      if (dataToSend != null)
      {
        content = new ByteArrayContent(dataToSend);
      }

      var result = await _httpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(result.StatusCode);
      }

      return HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode);
    }

    public async Task<bool> HttpPutBytesWithoutResponseAsync(string uri, byte[] dataToSend)
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await _httpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
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
