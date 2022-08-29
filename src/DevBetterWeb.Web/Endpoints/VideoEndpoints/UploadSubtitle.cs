using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiOrRoleAuthorization(AuthConstants.Roles.ADMINISTRATORS)]
public class UploadSubtitle : EndpointBaseAsync
	.WithRequest<UploadSubtitleRequest>
	.WithResult<HttpResponse<bool>>
{
	private readonly UploadVideoSubtitleService _uploadVideoSubtitleService;

	public UploadSubtitle(UploadVideoSubtitleService uploadVideoSubtitleService)
	{
		_uploadVideoSubtitleService = uploadVideoSubtitleService;
	}

	[HttpPost("videos/upload-subtitle")]
	public override async Task<HttpResponse<bool>> HandleAsync([FromBody] UploadSubtitleRequest request, CancellationToken cancellationToken = default)
	{
		var uploadVideoSubtitleRequest = new UploadVideoSubtitleRequest(request.VideoId, request.Subtitle, request.Language);
		var response = await _uploadVideoSubtitleService.ExecuteAsync(uploadVideoSubtitleRequest, cancellationToken);
		
		return response;
	}
}
