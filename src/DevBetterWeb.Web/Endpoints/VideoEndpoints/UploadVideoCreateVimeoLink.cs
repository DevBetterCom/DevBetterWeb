using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Ardalis.ApiEndpoints;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;
using DevBetterWeb.Core;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiOrRoleAuthorization(AuthConstants.Roles.ADMINISTRATORS)]
public class UploadVideoCreateVimeoLink : EndpointBaseAsync
	.WithRequest<UploadVideoResumableInfo>
	.WithResult<ActionResult<UploadVideoResumableInfo>>
{
	private readonly UploadResumableCreateVideoLinkService _uploadResumableCreateVideoLinkService;

	public UploadVideoCreateVimeoLink(UploadResumableCreateVideoLinkService uploadResumableCreateVideoLinkService)
	{
		_uploadResumableCreateVideoLinkService = uploadResumableCreateVideoLinkService;
	}

	[HttpPost("videos/upload-video-create-url")]
	public override async Task<ActionResult<UploadVideoResumableInfo>> HandleAsync([FromBody] UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var result = await _uploadResumableCreateVideoLinkService.ExecuteAsync(request, cancellationToken);

		return Ok(result.Data);
	}
}
