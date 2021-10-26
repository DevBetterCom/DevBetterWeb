using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class GetStatusAnimatedThumbnailServiceBuilder
  {
    public static GetStatusAnimatedThumbnailService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<GetStatusAnimatedThumbnailService>>().Object;

      return new GetStatusAnimatedThumbnailService(httpService, logger);
    }
  }
}
