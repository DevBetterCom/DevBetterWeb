using System.Net.Http;
using System.Threading.Tasks;
using DevBetterWeb.Web;
using Xunit;

namespace DevBetterWeb.FunctionalTests.RazorPages;

public class HomePageGet : IClassFixture<CustomWebApplicationFactory<Startup>>
{
  private readonly HttpClient _client;

  public HomePageGet(CustomWebApplicationFactory<Startup> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsViewWithCorrectMessage()
  {
    HttpResponseMessage response = await _client.GetAsync("/");
    response.EnsureSuccessStatusCode();
    string stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("Steve has been a great coach and a source of precious practical advice and encouragement.", stringResponse);
  }
}
