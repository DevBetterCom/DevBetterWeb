using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class DeleteAllVideos
  {
    [Fact]
    public async Task ReturnsTrueDeleteAllVideosAsync()
    {
      var vimeoCredential = new VimeoCredential("TOKEN");
      var vimeoService = new VimeoService(vimeoCredential);
      var result = await vimeoService.DeleteAllVideosAsync();

      Assert.True(result);
    }
  }
}
