using System;
using System.Net.Http;
using DevBetterWeb.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using NimblePros.Vimeo.Constants;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Web;

public class VimeoServiceCollectionExtensionsTests
{
	[Theory]
	[InlineData(ServiceConstants.VIMEO_API_HTTP_NAME)]
	[InlineData(ServiceConstants.VIMEO_API_TUS_HTTP_NAME)]
	[InlineData(ServiceConstants.VIMEO_OEMBED_HTTP_NAME)]
	public void AddDevBetterVimeoServices_RemovesServiceDefaultsResilienceFromVimeoClient(string clientName)
	{
		using var provider = BuildProviderLikeProgram();

		HasResilienceHandler(provider, clientName).ShouldBeFalse();
	}

	[Theory]
	[InlineData(ServiceConstants.VIMEO_API_HTTP_NAME)]
	[InlineData(ServiceConstants.VIMEO_API_TUS_HTTP_NAME)]
	[InlineData(ServiceConstants.VIMEO_OEMBED_HTTP_NAME)]
	public void AddDevBetterVimeoServices_KeepsLongVimeoClientTimeout(string clientName)
	{
		using var provider = BuildProviderLikeProgram();

		var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient(clientName);

		client.Timeout.ShouldBe(TimeSpan.FromMinutes(60));
	}

	[Fact]
	public void AddDevBetterVimeoServices_LeavesResilienceOnOtherClients()
	{
		using var provider = BuildProviderLikeProgram();

		HasResilienceHandler(provider, "SomeOtherClient").ShouldBeTrue();
	}

	private static ServiceProvider BuildProviderLikeProgram()
	{
		var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
		builder.AddServiceDefaults();
		builder.Services.AddDevBetterVimeoServices("test-token");
		return builder.Services.BuildServiceProvider();
	}

	private static bool HasResilienceHandler(IServiceProvider provider, string clientName)
	{
		HttpMessageHandler? handler = provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(clientName);
		while (handler is DelegatingHandler delegating)
		{
			if (delegating is ResilienceHandler)
			{
				return true;
			}
			handler = delegating.InnerHandler;
		}
		return false;
	}
}
