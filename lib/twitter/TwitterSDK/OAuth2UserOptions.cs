namespace TwitterSDK.Examples;

public class OAuth2UserOptions
{
	public string? ClientId { get; set; }
	public string? ClientSecret { get; set; }
	public string? Callback { get; set; }
	public IEnumerable<OAuth2Scope>? Scopes { get; set; }
}
