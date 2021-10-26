using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class DeleteVideoServiceBuilder
  {
    public static DeleteVideoService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<DeleteVideoService>>().Object;

      return new DeleteVideoService(httpService, logger);
    }
  }
}
