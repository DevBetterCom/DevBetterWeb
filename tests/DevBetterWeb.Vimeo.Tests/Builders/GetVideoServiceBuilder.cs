using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
{
  public class GetVideoServiceBuilder
  {
    public static GetVideoService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<GetVideoService>>().Object;

      return new GetVideoService(httpService, logger);
    }
  }
}
