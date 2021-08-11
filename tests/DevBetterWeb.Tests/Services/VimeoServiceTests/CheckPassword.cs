using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class CheckPassword
  {
    [Fact]
    public async Task ReturnsTrueCheckPasswordAsync()
    {
      var vimeoCredential = new VimeoCredential("PVT");
      var vimeoService = new VimeoService(vimeoCredential);
      var uploadTicket = await vimeoService.CheckPasswordAsync("585903732", "122324");

      Assert.True(true);
    }
  }
}
