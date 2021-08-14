using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
{
  public class UserDetailsServiceBuilder
  {
    public static UserDetailsService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<UserDetailsService>>().Object;

      return new UserDetailsService(httpService, logger);
    }
  }
}
