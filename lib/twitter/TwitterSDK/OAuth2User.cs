using System.Security.Cryptography;
using System.Text;

namespace TwitterSDK.Examples;

public class OAuth2User : AuthClient
{
	private readonly OAuth2UserOptions _options;
	private string? _codeVerifier;
	private string? _codeChallenge;

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

		if (String.IsNullOrEmpty(callback)) throw new ArgumentException("Callback is required");
		if (scopes is null || !scopes.Any()) throw new ArgumentException("Scopes are required");

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
			$"code_challenge_method=s256&" +
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
}
