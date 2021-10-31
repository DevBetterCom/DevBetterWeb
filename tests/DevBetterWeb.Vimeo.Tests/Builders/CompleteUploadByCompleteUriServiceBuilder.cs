using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBetterWeb.Vimeo.Tests.Builders
{
  public class CompleteUploadByCompleteUriServiceBuilder
  {
    public static CompleteUploadByCompleteUriService Build(HttpService httpService)
    {
      var logger = new Mock<ILogger<CompleteUploadByCompleteUriService>>().Object;

      return new CompleteUploadByCompleteUriService(httpService, logger);
    }
  }
}
