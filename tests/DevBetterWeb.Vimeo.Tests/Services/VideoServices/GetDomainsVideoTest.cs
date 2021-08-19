using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class GetDomainsVideoServiceTest
  {
    private readonly TestFileHelper _testFileHelper;
    private readonly GetDomainsVideoService _getDomainsVideoService;

    public GetDomainsVideoServiceTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getDomainsVideoService = GetDomainsVideoServiceBuilder.Build(httpService);
      _testFileHelper = new TestFileHelper();
    }

    [Fact]
    public async Task ReturnsDomainsVideoTest()
    {
      var videoId = await _testFileHelper.UploadTest();

      var response = await _getDomainsVideoService
        .ExecuteAsync(videoId.ToString());      

      await _testFileHelper.DeleteTestFile(videoId.ToString());

      response.Data.ShouldNotBe(null);
    }
  }
}
