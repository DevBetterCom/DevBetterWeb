using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class GetOEmbedVideoTest
  {
    private readonly TestFileHelper _testFileHelper;
    private readonly GetOEmbedVideoService _getOEmbedVideoService;

    public GetOEmbedVideoTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getOEmbedVideoService = GetOEmbedVideoServiceBuilder.Build(httpService);
      _testFileHelper = new TestFileHelper();
    }

    [Fact]
    public async Task ReturnsOEmbedVideoTest()
    {
      var videoId = await _testFileHelper.UploadTest();

      videoId.ShouldNotBe(0);
      var videoLink = $"https://vimeo.com/videos/{videoId}";

      var response = await _getOEmbedVideoService
        .ExecuteAsync(videoLink);

      await _testFileHelper.DeleteTestFile(videoId.ToString());

      response.Code.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }
  }
}
