using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders;

public class GetPagedVideosServiceBuilder
{
  public static GetPagedVideosService Build(HttpService httpService)
  {
    var logger = new Mock<ILogger<GetPagedVideosService>>().Object;

    return new GetPagedVideosService(httpService, logger);
  }
}
