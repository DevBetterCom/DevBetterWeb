using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using DevBetterWeb.Vimeo.Tests.Constants;
using DevBetterWeb.Vimeo.Tests.Helpers;
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
      var httpService = HttpServiceBuilder.Build();
      _accountDetailsService = AccountDetailsServiceBuilder.Build(httpService);
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
