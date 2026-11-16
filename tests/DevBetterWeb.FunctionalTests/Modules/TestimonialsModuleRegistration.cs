using System.Threading.Tasks;
using DevBetterWeb.Modules.Testimonials.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevBetterWeb.FunctionalTests.Modules;

public class TestimonialsModuleRegistration : IClassFixture<CustomWebApplicationFactory<Program>>
{
	private readonly CustomWebApplicationFactory<Program> _factory;

	public TestimonialsModuleRegistration(CustomWebApplicationFactory<Program> factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task HostResolvesTestimonialQueriesAgainstItsDatabase()
	{
		using var scope = _factory.Services.CreateScope();

		var queries = scope.ServiceProvider.GetRequiredService<ITestimonialQueries>();
		var accepted = await queries.ListAcceptedAsync();

		Assert.Empty(accepted);
	}
}
