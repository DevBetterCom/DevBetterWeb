using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class GetReadAccessUrl
  {
    [Fact]
    public void GetReadAccessUrlIsExpected()
    {
      var vimeoCredential = new VimeoCredential("client id", "client secret", "https://your_website_here.com/wherever-you-send-users-after-grant");
      var vimeoService = new VimeoService(vimeoCredential);
      //TODO: still in progress
      //var readAccessToken = await vimeoService.GetReadAccessTokenAsync();

      Assert.True(true);
    }
  }
}
