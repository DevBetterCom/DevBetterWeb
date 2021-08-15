using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class GetOEmbedVideoServiceBuilder
  {
    public static GetOEmbedVideoService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<GetOEmbedVideoService>>().Object;

      return new GetOEmbedVideoService(httpService, logger);
    }
  }
}
