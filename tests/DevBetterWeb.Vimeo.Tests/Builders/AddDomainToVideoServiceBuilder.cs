using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
{
  public class AddDomainToVideoServiceBuilder
  {
    public static AddDomainToVideoService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<AddDomainToVideoService>>().Object;

      return new AddDomainToVideoService(httpService, logger);
    }
  }
}
