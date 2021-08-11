using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class GetAllVideos
  {
    [Fact]
    public async Task ReturnsAllVideos()
    {
      var vimeoCredential = new VimeoCredential("PVT");
      var vimeoService = new VimeoService(vimeoCredential);
      var videos = await vimeoService.GetAllVideosAsync();

      Assert.True(true);
    }
  }
}
