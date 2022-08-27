using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.Pages.Admin.Videos;
using Microsoft.AspNetCore.Authorization;

namespace DevBetterWeb.Web.Endpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class UpdateDescription : EndpointBaseAsync
	.WithRequest<UpdateDescriptionRequest>
	.WithActionResult
{
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;
	private readonly GetVideoService _getVideoService;
	private readonly GetOEmbedVideoService _getOEmbedVideoService;
	private readonly IMarkdownService _markdownService;

	public UpdateDescription(IRepository<ArchiveVideo> archiveVideoRepository, GetVideoService getVideoService, GetOEmbedVideoService getOEmbedVideoService, IMarkdownService markdownService)
	{
		_archiveVideoRepository = archiveVideoRepository;
		_getVideoService = getVideoService;
		_getOEmbedVideoService = getOEmbedVideoService;
		_markdownService = markdownService;
	}

	[HttpPost("videos/update-description")]
	public override async Task<ActionResult> HandleAsync([FromForm] UpdateDescriptionRequest updateDescription, CancellationToken cancellationToken = default)
	{
		var video = await _getVideoService.ExecuteAsync(updateDescription.VideoId, cancellationToken);
		if (video?.Data == null) return NotFound($"Video Not Found {updateDescription.VideoId}");

		var oEmbed = await _getOEmbedVideoService.ExecuteAsync(video.Data.Link, cancellationToken);
		if (oEmbed?.Data == null) return NotFound($"Video Not Found {updateDescription.VideoId}");

		var spec = new ArchiveVideoByVideoIdSpec(updateDescription.VideoId!);
		var archiveVideo = await _archiveVideoRepository.FirstOrDefaultAsync(spec, cancellationToken);
		if (archiveVideo == null)
		{
			return NotFound($"Video Not Found {updateDescription.VideoId}");
		}

		archiveVideo.Description = updateDescription.Description;
		await _archiveVideoRepository.UpdateAsync(archiveVideo, cancellationToken);
		await _archiveVideoRepository.SaveChangesAsync(cancellationToken);

		var oEmbedViewModel = new OEmbedViewModel(oEmbed.Data);
		oEmbedViewModel.VideoId = int.Parse(archiveVideo.VideoId!);
		oEmbedViewModel.DescriptionMd = _markdownService.RenderHTMLFromMD(archiveVideo.Description);
		oEmbedViewModel.Description = archiveVideo.Description;
		oEmbedViewModel
			.BuildHtml(video.Data.Link);

		return Ok(oEmbedViewModel);
	}
}
