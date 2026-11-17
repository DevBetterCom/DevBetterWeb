using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#nullable enable

namespace DevBetterWeb.FunctionalTests.Auth;

/// <summary>
/// Signs requests in as the user named in <see cref="UserHeader"/> with the role in <see cref="RoleHeader"/>.
/// Requests without the header are anonymous, so [Authorize] pages return 401 (the challenge).
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public const string SchemeName = "Test";
	public const string UserHeader = "X-Test-User";
	public const string RoleHeader = "X-Test-Role";

	public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
		: base(options, logger, encoder)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		string? userName = Request.Headers[UserHeader];
		if (string.IsNullOrEmpty(userName))
		{
			return Task.FromResult(AuthenticateResult.NoResult());
		}

		var claims = new[]
		{
			new Claim(ClaimTypes.Name, userName),
			new Claim(ClaimTypes.Role, Request.Headers[RoleHeader].ToString()),
		};
		var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
		return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
	}
}
