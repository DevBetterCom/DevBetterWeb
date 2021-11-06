using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders;

public class GetAnimatedThumbnailServiceBuilder
{
  public static GetAnimatedThumbnailService Build(HttpService httpService)
  {
    var logger = new Mock<ILogger<GetAnimatedThumbnailService>>().Object;

    return new GetAnimatedThumbnailService(httpService, logger);
  }
}
