using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadResumableCreateVideoLinkService : BaseAsyncApiCaller
  .WithRequest<UploadVideoResumableInfo>
  .WithResponse<UploadVideoResumableInfo>
{
	private readonly HttpService _httpService;
	private readonly HttpClient _httpClient;
  private readonly ILogger<UploadResumableCreateVideoLinkService> _logger;

  public UploadResumableCreateVideoLinkService(
		HttpService httpService,
	  HttpClient httpClient,
    ILogger<UploadResumableCreateVideoLinkService> logger)
  {
	  _httpService = httpService;
	  _httpClient = httpClient;
    _logger = logger;
  }

  public override async Task<HttpResponse<UploadVideoResumableInfo>> ExecuteAsync(UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
  {
    try
    {
			var uploadTus = new UploadTusRequest(request.FileFullSize);
			var response = await _httpService.HttpPostAsync<VideoForCreateResumableLink>($"{ServiceConstants.ME}/videos", uploadTus);
			if (response.Code != HttpStatusCode.OK && response.Code != HttpStatusCode.Created)
			{
				return new HttpResponse<UploadVideoResumableInfo>(response.Code);
			}

			request.UploadUrl = response.Data?.Upload?.UploadLink;
			request.UploadOffset = 0;
			request.VideoId = response.Data?.VideoId;
			request.VideoUrl = response.Data?.Uri;

			return new HttpResponse<UploadVideoResumableInfo>(request, HttpStatusCode.OK);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<UploadVideoResumableInfo>.FromException(exception.Message);
    }
  }
}
