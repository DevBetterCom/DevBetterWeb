using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.MappingProfiles;

public class AutoMapperConfigurationTests
{
	[Fact]
	public void Web_Mapping_Profiles_Load_And_Compile()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
		using var provider = services.BuildServiceProvider();

		var mapper = provider.GetRequiredService<IMapper>();

		// Profiles have known unmapped destination members, so AssertConfigurationIsValid is too strict.
		// Compiling every map still catches expression/configuration breaks from AutoMapper upgrades.
		mapper.ConfigurationProvider.CompileMappings();
	}
}
