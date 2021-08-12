using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Constants;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class AccountDetailsTest
  {
    [Fact]
    public async Task ReturnsAccountDetailsTest()
    {
      var accountDetailsService = new AccountDetailsService(null, null, null);
      var user = await accountDetailsService
        .SetToken(AccountConstants.ACCESS_TOKEN)
        .ExecuteAsync();

      user.Data.Account.ShouldBe("");
    }
  }
}
