using DevBetterWeb.Core.Entities;
using System;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class GetFullAccessUrl
  {
    [Fact]
    public async Task GetFullAccessUrlIsExpected()
    {
      var vimeoCredential = new VimeoCredential("client id", "client secret", "https://your_website_here.com/wherever-you-send-users-after-grant");
      var vimeoService = new VimeoService(vimeoCredential);
      var fullAccessToken = await vimeoService.GetFullAccessTokenAsync();

      Assert.True(true);
    }
  }
}
