using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class GetUnauthenticatedToken
  {
    [Fact]
    public async Task GetUnauthenticatedTokenIsExpected()
    {
      var vimeoCredential = new VimeoCredential("client id", "client secret", "https://your_website_here.com/wherever-you-send-users-after-grant");
      var vimeoService = new VimeoService(vimeoCredential);
      var readAccessToken = await vimeoService.GetUnauthenticatedToken();

      Assert.True(true);
    }
  }
}
