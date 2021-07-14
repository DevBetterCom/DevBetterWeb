using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class GetFullAccessUrl
  {
    [Fact]
    public void GetFullAccessUrlIsExpected()
    {
      var vimeoCredential = new VimeoCredential("client id", "client secret", "https://your_website_here.com/wherever-you-send-users-after-grant");
      var vimeoService = new VimeoService(vimeoCredential);

      //TODO: still in progress
      //var fullAccessToken = await vimeoService.GetFullAccessTokenAsync();

      Assert.True(true);
    }
  }
}
