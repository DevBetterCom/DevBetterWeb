using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
{
  public class GetDomainsVideoServiceBuilder
  {
    public static GetDomainsVideoService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<GetDomainsVideoService>>().Object;

      return new GetDomainsVideoService(httpService, logger);
    }
  }
}
