using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[IntegrationApiAuthorization]
public class DeleteAllVideosNoVimeo : EndpointBaseAsync
	.WithoutRequest
	.WithoutResult
{
	private readonly IVideosService _videosService;

	public DeleteAllVideosNoVimeo(IVideosService videosService)
	{
		_videosService = videosService;
	}

	[HttpGet("videos/delete-all-videos-no-vimeo")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		await _videosService.DeleteVideosNotExistOnVimeoFromVimeo(null);
		await _videosService.DeleteVideosNotExistOnVimeoFromDatabase(null);

		return Ok();
	}
}
