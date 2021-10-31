using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class DeleteVideoTest
  {
    private readonly DeleteVideoService _deleteVideoService;
    private readonly TestFileHelper _testFileHelper;

    public DeleteVideoTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _deleteVideoService = DeleteVideoServiceBuilder.Build(httpService);
      _testFileHelper = new TestFileHelper();
    }

    [Fact]
    public async Task ReturnsSuccessAccountDetailsTest()
    {
      var videoId = await _testFileHelper.UploadTest();

      videoId.ShouldNotBe(0);

      var result = await _testFileHelper.DeleteTestFile(videoId.ToString());

      result.ShouldBe(System.Net.HttpStatusCode.NoContent);
    }
  }
}
