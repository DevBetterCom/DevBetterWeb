using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;
using NimblePros.Vimeo.VideoServices;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
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

		var deleteVideoRequest = new DeleteVideoRequest(long.Parse(vimeoVideoId));
		var deleteResponse = await _deleteVideoService.ExecuteAsync(deleteVideoRequest, cancellationToken);

		if (deleteResponse != null && deleteResponse.IsSuccess)
		{
			return NoContent();
		}

		// something went wrong
		throw new System.Exception(deleteResponse!.Message);
	}
}
