using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;
using NimblePros.Vimeo.VideoServices;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
public class DeleteVideo : EndpointBaseAsync
	.WithRequest<string>
	.WithoutResult
{
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly DeleteVideoService _deleteVideoService;
	private readonly IVideosCacheService _videosCacheService;

	public DeleteVideo(IRepository<ArchiveVideo> repository, DeleteVideoService deleteVideoService, IVideosCacheService videosCacheService)
	{
		_repository = repository;
		_deleteVideoService = deleteVideoService;
		_videosCacheService = videosCacheService;
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
		_ = _videosCacheService.UpdateAllVideosAsync();

		return (ActionResult)deleteResponse.ActionResult;
	}
}
