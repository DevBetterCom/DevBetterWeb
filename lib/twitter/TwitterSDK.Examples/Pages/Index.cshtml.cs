using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace TwitterSDK.Examples.Pages;

public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;
	private readonly OAuth2User _authClient;
	private const string STATE = "my-state";

	public string TwitterAuthURL { get; set; } = "";

	public IndexModel(ILogger<IndexModel> logger, IOptions<TwitterSettings> twitterSettings)
	{
		_logger = logger;
		_authClient = new OAuth2User(new OAuth2UserOptions
		{
			ClientId = twitterSettings.Value.ClientId,
			ClientSecret = twitterSettings.Value.ClientSecret,
			Callback = "https://127.0.0.1:7179/callback",
			Scopes = new List<OAuth2Scope>()
			{
				OAuth2Scope.TweetRead, OAuth2Scope.UsersRead, OAuth2Scope.OfflineAccess
			}
		});
	}

	public void OnGet()
	{
		TwitterAuthURL = _authClient.GenerateAuthUrlS256(STATE);
	}
}
