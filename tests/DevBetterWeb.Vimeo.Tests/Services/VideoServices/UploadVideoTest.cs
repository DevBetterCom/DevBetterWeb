using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Constants;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class UploadVideoTest
  {
    private readonly TestFileHelper _testFileHelper;
    private readonly UploadVideoService _uploadVideoService;

    public UploadVideoTest()
    {
      _testFileHelper = new TestFileHelper();
      var httpService = HttpServiceBuilder.Build();
      _uploadVideoService = UploadVideoServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsSuccessUploadVideoTest()
    {
      var buffer = await TestFileHelper.BuildAsync();

      var video = new Video();
      video.Name = "Test";
      video.Description = "Test";
      video.Privacy.View = PrivacyViewType.DISABLE_TYPE;
      video.Privacy.Embed = AccessType.WHITELIST_TYPE;
      video.Privacy.Download = false;

      var request = new UploadVideoRequest("me", buffer, video, ConfigurationConstants.VIMEO_ALLOWED_DOMAIN);

      request.FileData = buffer;

      var response = await _uploadVideoService
        .ExecuteAsync(request);

      await _testFileHelper.DeleteTestFile(response.Data.ToString());

      response.Data.ShouldNotBe(0);      
    }    
  }
}
