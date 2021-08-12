using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Constants;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class AccountDetailsTest
  {
    private readonly AccountDetailsService _accountDetailsService;

    public AccountDetailsTest()
    {
      var loggerUserDetailsService = new Mock<ILogger<UserDetailsService>>().Object;
      var loggerAccountDetailsService = new Mock<ILogger<AccountDetailsService>>().Object;
      var httpClient = new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) };
      var httpService = new HttpService(httpClient);
      var userDetailsService = new UserDetailsService(httpService, loggerUserDetailsService);

      _accountDetailsService = new AccountDetailsService(httpService, loggerAccountDetailsService, userDetailsService);
    }

    [Fact]
    public async Task ReturnsAccountDetailsTest()
    {
      var user = await _accountDetailsService
        .SetToken(AccountConstants.ACCESS_TOKEN)
        .ExecuteAsync();

      user.Data.Account.ShouldBe("");
    }
  }
}
