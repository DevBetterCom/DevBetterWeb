using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class AccountDetailsTest
  {
    private readonly AccountDetailsService _accountDetailsService;

    public AccountDetailsTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _accountDetailsService = AccountDetailsServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsAccountDetailsTest()
    {
      var user = await _accountDetailsService
        .ExecuteAsync();

      user.Data.ShouldNotBe(null);
      user.Code.ShouldBe(System.Net.HttpStatusCode.OK);
    }
  }
}
