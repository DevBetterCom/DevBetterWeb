using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.WpfUploader.ApiClients;
using DevBetterWeb.WpfUploader.Models;
using MediaInfo;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DevBetterWeb.WpfUploader.Services;

// TODO: Refactor - this is way too big and has too many responsibilities and abstraction levels in it.
public class GetVideosService
{
  private readonly GetAllVideosService _getAllVideosService;
  private readonly ILogger<GetVideosService> _logger;
  private readonly ConfigInfo _configInfo;

  public GetVideosService(
    ConfigInfo configInfo,
    HttpService httpService,
    GetAllVideosService getAllVideosService,
    ILogger<GetVideosService> logger)
  {
    _configInfo = configInfo;
    httpService.SetAuthorization(_configInfo.Token);
    _getAllVideosService = getAllVideosService;
    _logger = logger;
  }

  public async Task<List<Video>> GetAllVimeoVideosAsync()
  {
    HttpResponse<DataPaged<Video>> allVideosResponse;
    var videos = new List<Video>();

    var pageNumber = 1;
    do
    {
      var getAllRequest = new GetAllVideosRequest(ServiceConstants.ME, pageNumber);
      allVideosResponse = await _getAllVideosService.ExecuteAsync(getAllRequest);
      if (allVideosResponse != null && allVideosResponse.Data != null)
      {
        videos.AddRange(allVideosResponse.Data.Data);
      }
      pageNumber++;
    } while (allVideosResponse != null && allVideosResponse.Data != null);

    return videos;
  }
}
