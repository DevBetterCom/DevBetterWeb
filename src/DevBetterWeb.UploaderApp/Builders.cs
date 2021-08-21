using System;
using System.Net.Http;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.UploaderApp
{
  public static class Builders
  {
    private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
    {
      builder.SetMinimumLevel(LogLevel.Information);
    });

    public static UploadVideoService BuildUploadVideoService(HttpService httpService)
    {
      var updateVideoDetailsService = BuildUpdateVideoDetailsService(_loggerFactory, httpService);
      var getStreamingTicketService = BuildGetStreamingTicketService(_loggerFactory, httpService);
      var completeUploadByCompleteUriService = BuildCompleteUploadByCompleteUriService(_loggerFactory, httpService);
      var addDomainToVideoService = BuildAddDomainToVideoService(_loggerFactory, httpService);


      return new UploadVideoService(httpService, _loggerFactory.CreateLogger<UploadVideoService>(), getStreamingTicketService, completeUploadByCompleteUriService, updateVideoDetailsService, addDomainToVideoService);
    }

    public static GetAllVideosService BuildGetAllVideosService(HttpService httpService)
    {
      return new GetAllVideosService(httpService, _loggerFactory.CreateLogger<GetAllVideosService>());
    }

    public static HttpService BuildHttpService(string token)
    {
      var httpClient = new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) };
      httpClient.DefaultRequestHeaders.Remove("Accept");
      httpClient.DefaultRequestHeaders.Add("Accept", ServiceConstants.VIMEO_HTTP_ACCEPT);
      httpClient.Timeout = TimeSpan.FromMinutes(60);
      var httpService = new HttpService(httpClient);

      httpService.SetAuthorization(token);

      return httpService;
    }

    private static AddDomainToVideoService BuildAddDomainToVideoService(ILoggerFactory loggerFactory, HttpService httpService)
    {
      return new AddDomainToVideoService(httpService, loggerFactory.CreateLogger<AddDomainToVideoService>());
    }

    private static CompleteUploadByCompleteUriService BuildCompleteUploadByCompleteUriService(ILoggerFactory loggerFactory, HttpService httpService)
    {

      return new CompleteUploadByCompleteUriService(httpService, loggerFactory.CreateLogger<CompleteUploadByCompleteUriService>());
    }

    private static GetStreamingTicketService BuildGetStreamingTicketService(ILoggerFactory loggerFactory, HttpService httpService)
    {
      return new GetStreamingTicketService(httpService, loggerFactory.CreateLogger<GetStreamingTicketService>());
    }

    private static UpdateVideoDetailsService BuildUpdateVideoDetailsService(ILoggerFactory loggerFactory, HttpService httpService)
    {
      return new UpdateVideoDetailsService(httpService, loggerFactory.CreateLogger<UpdateVideoDetailsService>());
    }
  }
}
