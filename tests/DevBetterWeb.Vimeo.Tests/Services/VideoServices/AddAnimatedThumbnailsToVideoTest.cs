using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class AddAnimatedThumbnailsToVideoTest
  {
    private static int START_TIME = 0;
    private static int DURATION = 2;

    private readonly AddAnimatedThumbnailsToVideoService _addAnimatedThumbnailsToVideoService; 
      private readonly GetStatusAnimatedThumbnailService _getStatusAnimatedThumbnailService;
    private readonly GetAnimatedThumbnailService _getAnimatedThumbnailService;
    private readonly TestFileHelper _testFileHelper;

    public AddAnimatedThumbnailsToVideoTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _addAnimatedThumbnailsToVideoService = AddAnimatedThumbnailsToVideoServiceBuilder.Build(httpService);
      _getStatusAnimatedThumbnailService = GetStatusAnimatedThumbnailServiceBuilder.Build(httpService);
      _getAnimatedThumbnailService = GetAnimatedThumbnailServiceBuilder.Build(httpService);
      _testFileHelper = new TestFileHelper();
    }

    [Fact]
    public async Task ReturnsSuccessAddAnimatedThumbnailsTest()
    {     
      var videoId = await _testFileHelper.UploadTest();
      videoId.ShouldNotBe(0);

      Thread.Sleep(60*1000);
      var request = new AddAnimatedThumbnailsToVideoRequest(videoId, START_TIME, DURATION);
      var result = await _addAnimatedThumbnailsToVideoService.ExecuteAsync(request);
      result.ShouldNotBeNull();
      result.Code.ShouldNotBe(HttpStatusCode.BadRequest);

      var status = string.Empty;
      var getStatusAnimatedThumbnailRequest = new GetAnimatedThumbnailRequest(videoId, result.Data.PictureId);
      while(status != "completed")
      {
        var statusResult = await _getStatusAnimatedThumbnailService.ExecuteAsync(getStatusAnimatedThumbnailRequest);
        statusResult.Code.ShouldNotBe(HttpStatusCode.BadRequest);

        status = statusResult.Data.Status;
        Thread.Sleep(5*1000);
      }

      var getAnimatedThumbnailResult = await _getAnimatedThumbnailService.ExecuteAsync(getStatusAnimatedThumbnailRequest);
      getAnimatedThumbnailResult.Code.ShouldNotBe(HttpStatusCode.BadRequest);

      var deleteResult = await _testFileHelper.DeleteTestFile(videoId.ToString());

      deleteResult.ShouldBe(System.Net.HttpStatusCode.NoContent);
    }
  }
}
