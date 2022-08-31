namespace TwitterSDK.Examples;

public class OAuth2User : AuthClient
{
	private readonly OAuth2UserOptions _options;

	public OAuth2User(OAuth2UserOptions options)
	{
		_options = options;
	}
}
