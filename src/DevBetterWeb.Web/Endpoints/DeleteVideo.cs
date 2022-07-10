using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[IntegrationApiAuthorization]
public class DeleteVideo : EndpointBaseAsync
	.WithRequest<string>
	.WithoutResult
{
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly DeleteVideoService _deleteVideoService;

	public DeleteVideo(IRepository<ArchiveVideo> repository, DeleteVideoService deleteVideoService)
	{
		_repository = repository;
		_deleteVideoService = deleteVideoService;
	}

	[HttpDelete("videos/uploader/delete-video/{vimeoVideoId}")]
	public override async Task<ActionResult> HandleAsync([FromRoute] string vimeoVideoId, CancellationToken cancellationToken = default)
	{
		var spec = new ArchiveVideoByVideoIdSpec(vimeoVideoId);
		var existVideo = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
		if (existVideo != null)
		{
			await _repository.DeleteAsync(existVideo, cancellationToken);
		}

		await _deleteVideoService.ExecuteAsync(vimeoVideoId, cancellationToken);

		return Ok();
	}
}
