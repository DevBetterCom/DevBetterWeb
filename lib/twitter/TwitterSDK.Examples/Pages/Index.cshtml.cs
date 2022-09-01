using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace TwitterSDK.Examples.Pages;

public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;
	private readonly IOptions<TwitterSettings> _twitterSettings;
	static private OAuth2User? _authClient;
	private Client? _client;
	private const string STATE = "my-state";

	public string TwitterAuthURL { get; set; } = "";

	public IndexModel(ILogger<IndexModel> logger, IOptions<TwitterSettings> twitterSettings)
	{
		_logger = logger;
		_twitterSettings = twitterSettings;
	}

	public async Task OnGet()
	{
		if (_authClient is null) // First time visiting the page
		{
			_authClient = new OAuth2User(new OAuth2UserOptions
			{
				ClientId = _twitterSettings.Value.ClientId,
				ClientSecret = _twitterSettings.Value.ClientSecret,
				Callback = Request.Scheme + "://" + Request.Host + Request.PathBase,
				Scopes = new List<OAuth2Scope>()
			{
				OAuth2Scope.TweetRead, OAuth2Scope.UsersRead, OAuth2Scope.FollowsRead, OAuth2Scope.OfflineAccess
			}
			});
			_client = new Client(_authClient);

			TwitterAuthURL = _authClient.GenerateAuthUrlS256(STATE);
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

					await _authClient.RequestAccessToken(code);
					_logger.LogInformation("Looks like it worked and we got a token");
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
			}
		}	
	}
}
