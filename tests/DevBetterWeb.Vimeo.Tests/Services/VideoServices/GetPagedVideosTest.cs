using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests;

public class GetPagedVideosTest
{
  private readonly GetPagedVideosService _getPagedVideosService;

  public GetPagedVideosTest()
  {
    var httpService = HttpServiceBuilder.Build();
		_getPagedVideosService = GetPagedVideosServiceBuilder.Build(httpService);
  }

  [Fact]
  public async Task ReturnsPagedVideosTest()
  {
    var request = new GetAllVideosRequest("me");
    var response = await _getPagedVideosService
			.ExecuteAsync(request);

    response.Code.ShouldBe(System.Net.HttpStatusCode.OK);
    response.Data.Data.Count.ShouldBeGreaterThan(0);
  }
}
