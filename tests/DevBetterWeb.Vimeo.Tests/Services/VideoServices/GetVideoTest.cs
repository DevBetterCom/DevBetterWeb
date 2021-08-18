using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class GetVideoTest
  {
    private readonly TestFileHelper _testFileHelper;
    private readonly GetVideoService _getVideoService;

    public GetVideoTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getVideoService = GetVideoServiceBuilder.Build(httpService);
      _testFileHelper = new TestFileHelper();
    }

    [Fact]
    public async Task ReturnsVideoTest()
    {
      var videoId = await _testFileHelper.UploadTest();

      videoId.ShouldNotBe(0);

      var response = await _getVideoService
        .ExecuteAsync(videoId.ToString());

      await _testFileHelper.DeleteTestFile(response.Data.ToString());

      response.Data.ShouldNotBe(null);
    }
  }
}
