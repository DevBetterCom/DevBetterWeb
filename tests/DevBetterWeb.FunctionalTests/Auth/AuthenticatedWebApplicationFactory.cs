using System.Net.Http;
using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DevBetterWeb.FunctionalTests.Auth;

/// <summary>
/// The Testing host plus a header-driven authentication scheme, for pages that require a signed-in member.
/// </summary>
public class AuthenticatedWebApplicationFactory : CustomWebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		base.ConfigureWebHost(builder);
		builder.ConfigureTestServices(services =>
			services.AddAuthentication(TestAuthHandler.SchemeName)
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { }));
	}

	public HttpClient CreateClientFor(string userName, string role = AuthConstants.Roles.MEMBERS)
	{
		var client = CreateAnonymousClient();
		client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, userName);
		client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);
		return client;
	}

	public HttpClient CreateAnonymousClient() =>
		CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
}
