using System;
using System.Collections.Generic;
using System.Net.Http;
using DevBetterWeb.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevBetterWeb.UnitTests.Infrastructure.Vimeo;

// ServiceDefaults adds the standard resilience handler (30 second total timeout) to every
// HttpClient. Vimeo uploads are long-running, so the Vimeo clients must opt out of it.
public class VimeoHttpClientResilienceTests
{
	private const string ResilienceHandlerTypeName = "ResilienceHandler";

	private static IHttpMessageHandlerFactory BuildHandlerFactory()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.ConfigureHttpClientDefaults(http => http.AddStandardResilienceHandler());
		services.AddHttpClient("SomeOtherClient");
		services.RegisterVimeoServicesDependencies("test-token");

		return services.BuildServiceProvider().GetRequiredService<IHttpMessageHandlerFactory>();
	}

	private static IEnumerable<Type> GetHandlerChainTypes(HttpMessageHandler handler)
	{
		HttpMessageHandler? current = handler;
		while (current is not null)
		{
			yield return current.GetType();
			current = (current as DelegatingHandler)?.InnerHandler;
		}
	}

	[Fact]
	public void DefaultClientsGetResilienceHandler()
	{
		var factory = BuildHandlerFactory();

		var chain = GetHandlerChainTypes(factory.CreateHandler("SomeOtherClient"));

		Assert.Contains(chain, t => t.Name == ResilienceHandlerTypeName);
	}

	[Theory]
	[InlineData("VimeoApi")]
	[InlineData("VimeoOEmbed")]
	[InlineData("VimeoTusApi")]
	public void VimeoClientsDoNotGetResilienceHandler(string clientName)
	{
		var factory = BuildHandlerFactory();

		var chain = GetHandlerChainTypes(factory.CreateHandler(clientName));

		Assert.DoesNotContain(chain, t => t.Name == ResilienceHandlerTypeName);
	}
}
