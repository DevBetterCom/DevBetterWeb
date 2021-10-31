using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class AddDomainToVideoServiceTest
  {
    private readonly AddDomainToVideoService _addDomainToVideoService;
    private readonly TestFileHelper _testFileHelper;

    public AddDomainToVideoServiceTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _addDomainToVideoService = AddDomainToVideoServiceBuilder.Build(httpService);
      _testFileHelper = new TestFileHelper();
    }

    [Fact]
    public async Task ReturnsSuccessAddDomainToVideoTest()
    {
      var videoId = await _testFileHelper.UploadTest();

      videoId.ShouldNotBe(0);

      var response = await _addDomainToVideoService
        .ExecuteAsync(new AddDomainToVideoRequest(videoId, "localhost:5010"));

      await _testFileHelper.DeleteTestFile(videoId.ToString());
      
      response.Data.ShouldBeTrue();
    }
  }
}
