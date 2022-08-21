using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Extensions;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadResumableVideoService : BaseAsyncApiCaller
  .WithRequest<UploadVideoResumableInfo>
  .WithResponse<UploadVideoResumableInfo>
{
	private readonly HttpService _httpService;
	private readonly HttpClient _httpClient;
  private readonly ILogger<UploadResumableVideoService> _logger;

  public UploadResumableVideoService(
		HttpService httpService,
	  HttpClient httpClient,
    ILogger<UploadResumableVideoService> logger)
  {
	  _httpService = httpService;
	  _httpClient = httpClient;
    _logger = logger;
  }

  public override async Task<HttpResponse<UploadVideoResumableInfo>> ExecuteAsync(UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
  {
    try
    {
	    var uri = new Uri(request.UploadUrl);
			var baseAddress = uri.GetLeftPart(System.UriPartial.Authority);
			var path = request.UploadUrl.Replace(baseAddress + "/", "");
			_httpService.SetBaseUri(ServiceConstants.VIMEO_HTTP_ACCEPT, new Uri(baseAddress));

			_httpService.AddHeader("Tus-Resumable", "1.0.0");
			_httpService.AddHeader("Upload-Offset", request.UploadOffset.ToString());

			var byteContent = new ByteArrayContent(request.FilePartData, 0, request.PartSize);
			byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/offset+octet-stream");
			var response = await _httpService.HttpPatchBytesAsync(path, byteContent, cancellationToken);
			byteContent.Dispose();

			var uploadOffsetString = response.ResponseHeaders.GetValues("Upload-Offset")?.FirstOrDefault();

	    int.TryParse(uploadOffsetString, out var uploadOffset);
	    request.UploadOffset = uploadOffset;

			return new HttpResponse<UploadVideoResumableInfo>(request, HttpStatusCode.OK);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<UploadVideoResumableInfo>.FromException(exception.Message);
    }
  }
}
