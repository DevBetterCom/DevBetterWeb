using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NimblePros.Vimeo.VideoServices;
using NimblePros.Vimeo.VideoTusService;

namespace DevBetterWeb.Web.Endpoints.VideoEndpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class UploadChunkEndpoint : EndpointBaseAsync
	.WithRequest<UploadChunkRequest>
	.WithActionResult<UploadChunkStatus>
{
	private readonly ICreateVideoService _createVideo;
	private readonly VimeoSettings _vimeoSettings;

	public UploadChunkEndpoint(ICreateVideoService createVideo, VimeoSettings vimeoSettings)
	{
		_createVideo = createVideo;
		_vimeoSettings = vimeoSettings;
	}

	[HttpPost("api/videos/upload")]
	public override async Task<ActionResult<UploadChunkStatus>> HandleAsync([FromBody] UploadChunkRequest uploadChunkRequest, CancellationToken cancellationToken = default)
	{
		if (uploadChunkRequest.Chunk.Length <= 0)
		{
			return BadRequest("File can not be empty!");
		}

		var isBaseFolder = uploadChunkRequest.FolderId == null;
		var folderId = uploadChunkRequest.FolderId ?? _vimeoSettings.BaseFolderId;
		var result = await _createVideo.UploadChunkAsync(isBaseFolder, uploadChunkRequest.SessionId, uploadChunkRequest.Chunk, uploadChunkRequest.Description, folderId, cancellationToken);

		return Ok(result);
	}
}
