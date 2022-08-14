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

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
public class UploadVideo : EndpointBaseAsync
	.WithRequest<UploadVideoResumableInfo>
	.WithResult<ActionResult<UploadVideoResumableInfo>>
{
	private readonly UploadResumableVideoService _uploadResumableVideoService;

	public UploadVideo(UploadResumableVideoService uploadResumableVideoService)
	{
		_uploadResumableVideoService = uploadResumableVideoService;
	}

	[HttpPost("videos/upload-video")]
	public override async Task<ActionResult<UploadVideoResumableInfo>> HandleAsync([FromBody] UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var result = await _uploadResumableVideoService.ExecuteAsync(request, cancellationToken);

		return Ok(result?.Data);
	}
}
