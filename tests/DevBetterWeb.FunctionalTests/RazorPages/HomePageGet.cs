using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DevBetterWeb.FunctionalTests.RazorPages;

public class HomePageGet : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public HomePageGet(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsViewWithCorrectMessage()
  {
    HttpResponseMessage response = await _client.GetAsync("/");
    response.EnsureSuccessStatusCode();
    string stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("Steve has been a great coach and a source of precious practical", stringResponse);
  }
}
