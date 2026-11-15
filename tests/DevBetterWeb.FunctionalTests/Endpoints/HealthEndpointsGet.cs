using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DevBetterWeb.FunctionalTests.Endpoints;

public class HealthEndpointsGet : IClassFixture<CustomWebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public HealthEndpointsGet(CustomWebApplicationFactory<Program> factory)
	{
		_client = factory.CreateClient();
	}

	[Theory]
	[InlineData("/health")]
	[InlineData("/alive")]
	public async Task ReturnsHealthy(string path)
	{
		HttpResponseMessage response = await _client.GetAsync(path);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
	}
}
