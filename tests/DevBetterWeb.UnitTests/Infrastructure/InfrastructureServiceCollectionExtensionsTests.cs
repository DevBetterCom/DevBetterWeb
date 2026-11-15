using System.Linq;
using DevBetterWeb.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using NimblePros.ApiClient.Models;
using NimblePros.Vimeo.Extensions;
using NimblePros.Vimeo.Interfaces;
using NimblePros.Vimeo.Services.UtilityServices;
using NimblePros.Vimeo.VideoServices;
using NimblePros.Vimeo.VideoTusService;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Infrastructure;

public class InfrastructureServiceCollectionExtensionsTests
{
	[Theory]
	[InlineData("VimeoApi")]
	[InlineData("VimeoOEmbed")]
	[InlineData("VimeoTusApi")]
	public void AddInfrastructureServices_DoesNotConfigureVimeoHttpClient(string clientName)
	{
		// NimblePros.Vimeo 2.3.0+ throws at startup if these clients are configured before AddVimeoServices.
		var services = new ServiceCollection();
		services.AddOptions();

		services.AddInfrastructureServices(isDevelopment: false);

		using var provider = services.BuildServiceProvider();
		var options = provider.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>().Get(clientName);
		options.HttpClientActions.ShouldBeEmpty();
		options.HttpMessageHandlerBuilderActions.ShouldBeEmpty();
	}

	[Fact]
	public void AddInfrastructureServices_DoesNotRegisterVimeoServices()
	{
		var services = new ServiceCollection();

		services.AddInfrastructureServices(isDevelopment: false);

		services.ShouldNotContain(d => d.ServiceType == typeof(ClientApiConfigurations));
		services.ShouldNotContain(d => d.ServiceType == typeof(IUploadVideoTusService));
		services.ShouldNotContain(d => d.ServiceType == typeof(IUploadVideoSessionService));
		services.ShouldNotContain(d => d.ServiceType == typeof(ISleepService));
	}

	[Fact]
	public void AddInfrastructureServicesThenAddVimeoServices_RegistersVimeoServicesOnce()
	{
		var services = new ServiceCollection();
		services.AddLogging();

		services.AddInfrastructureServices(isDevelopment: false);
		services.AddVimeoServices("test-token");

		services.Count(d => d.ServiceType == typeof(ClientApiConfigurations)).ShouldBe(1);
		services.Count(d => d.ServiceType == typeof(IUploadVideoTusService)).ShouldBe(1);
		services.Count(d => d.ServiceType == typeof(IUploadVideoSessionService)).ShouldBe(1);
		services.Count(d => d.ServiceType == typeof(ISleepService)).ShouldBe(1);

		using var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();
		scope.ServiceProvider.GetRequiredService<GetVideoService>().ShouldNotBeNull();
		scope.ServiceProvider.GetRequiredService<IUploadVideoTusService>().ShouldNotBeNull();
	}
}
