using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
{
  public class UpdateVideoDetailsServiceBuilder
  {
    public static UpdateVideoDetailsService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<UpdateVideoDetailsService>>().Object;

      return new UpdateVideoDetailsService(httpService, logger);
    }
  }
}
