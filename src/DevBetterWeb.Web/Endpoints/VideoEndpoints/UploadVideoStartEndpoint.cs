using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Endpoints.VideoEndpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class UploadVideoStartEndpoint : EndpointBaseAsync
	.WithRequest<UploadVideoStartRequest>
	.WithResult<ActionResult<UploadVideoStartResponse>>
{
	private readonly ICreateVideoService _createVideo;

	public UploadVideoStartEndpoint(ICreateVideoService createVideo)
	{
		_createVideo = createVideo;
	}

	[HttpPost("videos/start")]
	public override async Task<ActionResult<UploadVideoStartResponse>> HandleAsync([FromBody] UploadVideoStartRequest request, CancellationToken cancellationToken = default)
	{
		string domain = HttpContext.Request.Host.Value;
		var sessionId = await _createVideo.StartAsync(request.VideoName, request.VideoSize, domain, cancellationToken);

		if (string.IsNullOrWhiteSpace(sessionId))
		{
			return BadRequest();
		}

		var response = new UploadVideoStartResponse { SessionId = sessionId };

		return Ok(response);
	}

}
