using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetAllVideosService : BaseAsyncApiCaller
  .WithRequest<GetAllVideosRequest>
  .WithResponse<List<Video>>
{
	private readonly ILogger<GetAllVideosService> _logger;
  private readonly GetPagedVideosService _getPagedVideosService;

  public GetAllVideosService(ILogger<GetAllVideosService> logger, GetPagedVideosService getPagedVideosService)
  {
	  _logger = logger;
    _getPagedVideosService = getPagedVideosService;
  }

  public override async Task<HttpResponse<List<Video>>> ExecuteAsync(GetAllVideosRequest request, CancellationToken cancellationToken = default)
  {
	  HttpResponse<DataPaged<Video>> allVideos;
	  var videosResult = new List<Video>();
	  var pageNumber = 1;
	  do
	  {
		  var getAllRequest = new GetAllVideosRequest(ServiceConstants.ME, pageNumber);
		  allVideos = await _getPagedVideosService.ExecuteAsync(getAllRequest, cancellationToken);
		  if (allVideos != null && allVideos.Data != null)
		  {
			  videosResult.AddRange(allVideos.Data.Data);
		  }
		  pageNumber++;
	  } while (allVideos != null && allVideos.Data != null);

	  return new HttpResponse<List<Video>>(videosResult, HttpStatusCode.OK);
  }
}
