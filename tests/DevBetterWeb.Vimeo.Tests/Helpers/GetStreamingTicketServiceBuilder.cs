using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class GetStreamingTicketServiceBuilder
  {
    public static GetStreamingTicketService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<GetStreamingTicketService>>().Object;

      return new GetStreamingTicketService(httpService, logger);
    }
  }
}
