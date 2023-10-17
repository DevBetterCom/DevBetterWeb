using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DevBetterWeb.Tests.Integration.Web;

public class QuestionsPageGet : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public QuestionsPageGet(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  //[Fact] Doesn't run on build server
  public async Task ReturnsViewWithCorrectMessage()
  {
    HttpResponseMessage response = await _client.GetAsync("/Questions");
    response.EnsureSuccessStatusCode();
    string stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("Questions Discussed", stringResponse);
  }
}
