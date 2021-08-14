﻿using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
{
  public class AccountDetailsServiceBuilder
  {
    public static AccountDetailsService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<AccountDetailsService>>().Object;

      var useDetails = UserDetailsServiceBuilder.Build(httpService);

      return new AccountDetailsService(httpService, logger, useDetails);
    }
  }
}
