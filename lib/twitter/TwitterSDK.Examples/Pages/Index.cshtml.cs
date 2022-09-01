using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace TwitterSDK.Examples.Pages;

public class IndexModel : PageModel
{
	static private string? _codeVerifier;
	static private string? _codeChallenge;

	public string TwitterAuthURL { get; set; } = "";

	private readonly IOptions<TwitterSettings> _twitterSettings;
	private readonly ILogger<IndexModel> _logger;
	private readonly HttpClient _httpClient = new HttpClient();


	public IndexModel(ILogger<IndexModel> logger, IOptions<TwitterSettings> twitterSettings)
	{
		_logger = logger;
		_twitterSettings = twitterSettings;
	}

	public async Task OnGet()
	{
		const string STATE = "my-state";
		string clientId = _twitterSettings.Value.ClientId;
		string clientSecret = _twitterSettings.Value.ClientSecret;
		string callback = Request.Scheme + "://" + Request.Host + Request.PathBase;

		if (_codeVerifier is null) // First time visiting the page
		{
			var scopes = new List<OAuth2Scope>()
			{
				OAuth2Scope.TweetRead, OAuth2Scope.UsersRead, OAuth2Scope.FollowsRead, OAuth2Scope.OfflineAccess
			};

			TwitterAuthURL = GenerateAuthUrlS256(STATE, clientId, callback, scopes);
		}
		else
		{
			try // Returning from Twitter auth
			{
				var code = Request.Query["code"];
				var state = Request.Query["state"];
				if (!String.IsNullOrEmpty(code) && !String.IsNullOrEmpty(state))
				{
					if (state != STATE)
					{
						ViewData["ErrorMessage"] = "State isn't matching";
						return;
					}

					Token token = await RequestAccessToken(code, clientId, clientSecret, callback, _codeVerifier);
					Guard.Against.NullOrEmpty(token.AccessToken, nameof(token.AccessToken));
					
					// Get user info
					var user = await FindMyUser(token.AccessToken);
					if (user is not null)
					{
						user["id"]
						user["name"]
						user["username"]
					}
					

					// Get followings
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
			}
		}
	}

	private string GenerateAuthUrlS256(
		string state,
		string clientId,
		string callback,
		IEnumerable<OAuth2Scope> scopes)
	{
		Random rnd = new Random();
		byte[] b = new byte[32];
		rnd.NextBytes(b);

		var codeVerifier = Base64URLEncode(b);
		_codeVerifier = codeVerifier;
		_codeChallenge = Base64URLEncode(Sha256(codeVerifier));

		return "https://twitter.com/i/oauth2/authorize?" +
			$"state={state}&" +
			$"client_id={clientId}&" +
			$"scope={String.Join(" ", scopes.Select(s => s.ToString()))}&" +
			$"response_type=code&" +
			$"redirect_uri={callback}&" +
			$"code_challenge_method=S256&" +
			$"code_challenge={_codeChallenge}";
	}

	private string Base64URLEncode(byte[] input)
	{
		return Convert.ToBase64String(input)
			.Replace('+', '-')
			.Replace('/', '_')
			.Replace("=", "");
	}

	private byte[] Sha256(string input)
	{
		return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
	}

	private async Task<Token> RequestAccessToken(
		string code,
		string clientId,
		string clientSecret,
		string callback,
		string codeVerifier)
	{
		var query = HttpUtility.ParseQueryString(String.Empty);
		query["code"] = code;
		query["grant_type"] = "authorization_code";
		query["code_verifier"] = codeVerifier;
		query["client_id"] = clientId;
		query["redirect_uri"] = callback;

		var headers = new Dictionary<string, string>()
		{
			["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret))
		};

		var data = await Rest<GetTokenResponse>(HttpMethod.Post, "/2/oauth2/token", query, headers);

		return ProcessTokenResponse(data);
	}

	private Token ProcessTokenResponse(GetTokenResponse token)
	{
		return new Token()
		{
			AccessToken = token.AccessToken,
			TokenType = token.TokenType,
			ExpiresAt = token.ExpiresIn is not null ? DateTime.Now.AddMilliseconds((int)token.ExpiresIn * 1000) : null,
			RefreshToken = token.RefreshToken,
			Scope = token.Scope
		};
	}

	private async Task<dynamic?> FindMyUser(string token)
	{
		var headers = new Dictionary<string, string>()
		{
			["Authorization"] = "Bearer " + token
		};

		var data = await Rest<dynamic>(HttpMethod.Get, "/2/users/me", null, headers);
		return data;
	}

	internal async Task<T?> Rest<T>(
	HttpMethod method,
	string endpoint,
	NameValueCollection? queryParams,
	Dictionary<string, string> headers)
	{
		var response = await MakeRequest(method, endpoint, queryParams, headers);
		string content = await response.Content.ReadAsStringAsync();
		var foo = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions() { });
		return foo;
	}

	internal async Task<HttpResponseMessage> MakeRequest(
		HttpMethod method,
		string endpoint,
		NameValueCollection? queryParams,
		Dictionary<string, string> headers)
	{
		var baseUrl = "https://api.twitter.com";
		var urlBuilder = new UriBuilder(baseUrl + endpoint);
		urlBuilder.Query = queryParams?.ToString();
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

	private async Task<HttpResponseMessage> SendWithRetriesAsync(HttpRequestMessage request, int maxRetries = 0)
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

internal class BaseToken
{
	[JsonPropertyName("refresh_token")]
	public string? RefreshToken { get; set; }
	[JsonPropertyName("access_token")]
	public string? AccessToken { get; set; }
	[JsonPropertyName("token_type")]
	public string? TokenType { get; set; }
	[JsonPropertyName("scope")]
	public string? Scope { get; set; }
}

internal class GetTokenResponse : BaseToken
{
	[JsonPropertyName("expires_in")]
	public int? ExpiresIn { get; set; }
}

internal class Token : BaseToken
{
	[JsonPropertyName("expires_at")]
	public DateTime? ExpiresAt { get; set; }
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
