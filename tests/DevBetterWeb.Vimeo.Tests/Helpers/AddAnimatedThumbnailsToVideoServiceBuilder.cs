using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class AddAnimatedThumbnailsToVideoServiceBuilder
  {
    public static AddAnimatedThumbnailsToVideoService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<AddAnimatedThumbnailsToVideoService>>().Object;

      return new AddAnimatedThumbnailsToVideoService(httpService, logger);
    }
  }
}
