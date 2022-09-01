using System.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;

namespace TwitterSDK.Examples;

internal class TwitterHttpClient
{
	private readonly HttpClient _httpClient = new HttpClient();

	internal async Task<T?> Rest<T>(
		HttpMethod method, 
		string endpoint, 
		NameValueCollection queryParams, 
		Dictionary<string, string> headers)
	{
		var response = await Request(method, endpoint, queryParams, headers);
		return await JsonSerializer.DeserializeAsync<T>(response.Content.ReadAsStream());
	}

	internal async Task<HttpResponseMessage> Request(
		HttpMethod method, 
		string endpoint, 
		NameValueCollection queryParams,
		Dictionary<string, string> headers)
	{
		var baseUrl = "https://api.twitter.com";
		var urlBuilder = new UriBuilder(baseUrl + endpoint);
		urlBuilder.Query = queryParams.ToString();
		var request = new HttpRequestMessage(method, urlBuilder.Uri);
		foreach (var header in headers)
		{
			request.Headers.Add(header.Key, header.Value);
		}
		request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
		
		HttpResponseMessage response = await SendWithRetriesAsync(request);

		if (response.StatusCode != HttpStatusCode.OK)
		{
			var error = await response.Content.ReadAsStringAsync();
			var json = JsonDocument.Parse(error);
			var description = json.RootElement.GetProperty("error_description").GetString();
			throw new TwitterResponseException(response.StatusCode, response.ReasonPhrase, response.Headers, description);
		}

		return response;
	}

	internal async Task<HttpResponseMessage> SendWithRetriesAsync(HttpRequestMessage request, int maxRetries = 0)
	{
		HttpResponseMessage res = await _httpClient.SendAsync(request);

		if (res.StatusCode == HttpStatusCode.TooManyRequests && maxRetries > 0)
		{
			long rateLimitReset = long.Parse(res.Headers.GetValues("X-Rate-Limit-Reset").First());
			long rateLimitRemaining = long.Parse(res.Headers.GetValues("X-Rate-Limit-Remaining").First());
			long timeTillReset = rateLimitReset * 1000 - DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var timeToWait = 1000L;
			if (rateLimitRemaining == 0)
			{
				timeToWait = timeTillReset;
			}
			await Task.Delay(TimeSpan.FromMilliseconds(timeToWait));
			return await SendWithRetriesAsync(request, maxRetries - 1);
		}

		return res;
	}
}

internal class TwitterResponseException : Exception
{
	internal HttpStatusCode Status { get; set; }
	internal string? StatusText { get; set; }
	internal HttpHeaders Headers { get; set; }
	internal string? ErrorDescription { get; set; }

	internal TwitterResponseException(
		HttpStatusCode status,
		string? statusText,
		HttpHeaders headers,
		string? errorDescription) : base(errorDescription)
	{
		Status = status;
		StatusText = statusText;
		Headers = headers;
		ErrorDescription = errorDescription;
	}
}
