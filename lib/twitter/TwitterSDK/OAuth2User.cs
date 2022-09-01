using System.Web;
using System.Security.Cryptography;
using System.Text;
using Ardalis.GuardClauses;

namespace TwitterSDK.Examples;

public class OAuth2User : AuthClient
{
	private Token? _token;
	private readonly OAuth2UserOptions _options;
	private string? _codeVerifier;
	private string? _codeChallenge;
	private TwitterHttpClient _httpClient = new TwitterHttpClient();

	public OAuth2User(OAuth2UserOptions options)
	{
		_options = options;
	}

	public override Task<IAuthHeader> GetAuthHeaderAsync(string? url, string? method)
	{
		throw new NotImplementedException();
	}

	public string GenerateAuthUrlS256(string state)
	{
		var clientId = _options.ClientId;
		var callback = _options.Callback;
		var scopes = _options.Scopes;

		Guard.Against.NullOrEmpty(callback, nameof(callback));
		Guard.Against.NullOrEmpty(scopes, nameof(scopes));

		Random rnd = new Random();
		byte[] b = new byte[32];
		rnd.NextBytes(b);

		var codeVerifier = Base64URLEncode(b);
		_codeVerifier = codeVerifier;
		Console.WriteLine($"codeVerifier: {codeVerifier}");
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

	public async Task RequestAccessToken(string code)
	{
		var clientId = _options.ClientId;
		var clientSecret = _options.ClientSecret;
		var callback = _options.Callback;
		var codeVerifier = _codeVerifier;

		Guard.Against.NullOrEmpty(clientId, nameof(clientId));
		Guard.Against.NullOrEmpty(callback, nameof(callback));

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

		var data = await _httpClient.Rest<GetTokenResponse>(HttpMethod.Post, "/2/oauth2/token", query, headers);

		_token = ProcessTokenResponse(data);
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

	public async Task<string> GetAuthHeader()
	{
		Guard.Against.NullOrEmpty(_token?.AccessToken, "AccessToken");
		if (IsAccessTokenExpired()) await RefreshAccessToken();
		return $"Bearer {_token.AccessToken}";
	}

	private Task RefreshAccessToken()
	{
		throw new NotImplementedException();
	}

	public bool IsAccessTokenExpired()
	{
		var refreshToken = _token?.RefreshToken;
		var expiresAt = _token?.ExpiresAt;
		return (
			refreshToken is null &&
			(expiresAt is not null || expiresAt <= DateTime.Now.AddMilliseconds(1000))
		);
	}
}

internal class BaseToken
{
	public string? RefreshToken { get; set; }
	public string? AccessToken { get; set; }
	public string? TokenType { get; set; }
	public string? Scope { get; set; }
}

internal class GetTokenResponse : BaseToken
{
	public int? ExpiresIn { get; set; }
}

internal class Token : BaseToken
{
	public DateTime? ExpiresAt { get; set; }
}
