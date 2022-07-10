using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
public class UpdateAllVideosThumbnails : EndpointBaseAsync
	.WithoutRequest
	.WithoutResult
{
	private readonly IVideosService _videosService;

	public UpdateAllVideosThumbnails(IVideosService videosService)
	{
		_videosService = videosService;
	}

	[HttpGet("videos/update-all-videos-thumbnails")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		await _videosService.UpdateVideosThumbnail(null);

		return Ok();
	}
}
