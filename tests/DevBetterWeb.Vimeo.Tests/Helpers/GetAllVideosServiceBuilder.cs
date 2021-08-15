using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class GetAllVideosServiceBuilder
  {
    public static GetAllVideosService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<GetAllVideosService>>().Object;

      return new GetAllVideosService(httpService, logger);
    }
  }
}
