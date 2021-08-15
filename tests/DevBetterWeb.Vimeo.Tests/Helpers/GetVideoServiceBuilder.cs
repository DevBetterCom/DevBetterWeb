using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
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
