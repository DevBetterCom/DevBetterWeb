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
	private static readonly string[] VimeoClientNames = ["VimeoApi", "VimeoOEmbed", "VimeoTusApi"];

	[Fact]
	public void AddInfrastructureServices_DoesNotRegisterVimeoServices()
	{
		var services = new ServiceCollection();

		services.AddInfrastructureServices(isDevelopment: false);

		ConfiguredHttpClientNames(services).ShouldNotContain(name => VimeoClientNames.Contains(name));
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

		// AddVimeoServices configures VimeoApi more than once itself (base settings, then the token),
		// so compare distinct names here; the guard against our own duplicate registration is the
		// AddInfrastructureServices test above.
		ConfiguredHttpClientNames(services).Distinct().ShouldBe(VimeoClientNames, ignoreOrder: true);
		services.Count(d => d.ServiceType == typeof(ClientApiConfigurations)).ShouldBe(1);
		services.Count(d => d.ServiceType == typeof(IUploadVideoTusService)).ShouldBe(1);
		services.Count(d => d.ServiceType == typeof(IUploadVideoSessionService)).ShouldBe(1);
		services.Count(d => d.ServiceType == typeof(ISleepService)).ShouldBe(1);

		using var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();
		scope.ServiceProvider.GetRequiredService<GetVideoService>().ShouldNotBeNull();
		scope.ServiceProvider.GetRequiredService<IUploadVideoTusService>().ShouldNotBeNull();
	}

	private static string[] ConfiguredHttpClientNames(IServiceCollection services) =>
		services
			.Where(d => d.ServiceType == typeof(IConfigureOptions<HttpClientFactoryOptions>))
			.Select(d => d.ImplementationInstance)
			.OfType<ConfigureNamedOptions<HttpClientFactoryOptions>>()
			.Select(o => o.Name ?? string.Empty)
			.ToArray();
}
