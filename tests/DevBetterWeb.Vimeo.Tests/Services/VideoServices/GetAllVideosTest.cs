using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Constants;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class GetAllVideosTest
  {
    private readonly GetAllVideosService _getAllVideosService;

    public GetAllVideosTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getAllVideosService = GetAllVideosServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsAllVideosTest()
    {
      var request = new GetAllVideosRequest("me");
      var response = await _getAllVideosService
        .ExecuteAsync(request);

      response.Code.ShouldBe(System.Net.HttpStatusCode.OK);
      response.Data.Data.Count.ShouldBeGreaterThan(0);
    }
  }
}
