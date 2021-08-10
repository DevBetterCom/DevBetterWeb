using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class GetAccountInformation
  {
    [Fact]
    public async Task ReturnsAccountInformationAsync()
    {
      var vimeoCredential = new VimeoCredential("pvt token");
      var vimeoService = new VimeoService(vimeoCredential);
      var user = await vimeoService.GetAccountInformationAsync();

      Assert.False(string.IsNullOrEmpty(user.Name));
    }
  }
}
