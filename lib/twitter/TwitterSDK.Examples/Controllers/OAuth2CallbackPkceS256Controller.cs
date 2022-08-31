using Microsoft.AspNetCore.Mvc;

namespace TwitterSDK.Examples.Controllers;

[ApiController]
[Route("[controller]")]
public class OAuth2CallbackPkceS256Controller : ControllerBase
{
	private static readonly string[] Summaries = new[]
	{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

	private readonly ILogger<OAuth2CallbackPkceS256Controller> _logger;

	public OAuth2CallbackPkceS256Controller(ILogger<OAuth2CallbackPkceS256Controller> logger)
	{
		_logger = logger;
	}

	[HttpGet(Name = "Login")]
	public RedirectResult Login()
	{
		var authUrl = 

		return Redirect();
	}
}
