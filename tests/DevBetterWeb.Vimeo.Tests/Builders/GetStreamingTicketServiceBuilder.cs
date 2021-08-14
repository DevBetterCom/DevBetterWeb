using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
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
