using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class UploadVideoServiceBuilder
  {
    public static UploadVideoService Build(HttpService httpService)
    {
      var loggerUploadVideoService = new Mock<ILogger<UploadVideoService>>().Object;

      var updateVideoDetailsService = UpdateVideoDetailsServiceBuilder.Build(httpService);
      var getStreamingTicketService = GetStreamingTicketServiceBuilder.Build(httpService);
      var completeUploadByCompleteUriService = CompleteUploadByCompleteUriServiceBuilder.Build(httpService);
      var addDomainToVideoService = AddDomainToVideoServiceBuilder.Build(httpService);


      return new UploadVideoService(httpService, loggerUploadVideoService, getStreamingTicketService, completeUploadByCompleteUriService, updateVideoDetailsService, addDomainToVideoService);
    }
  }
}
