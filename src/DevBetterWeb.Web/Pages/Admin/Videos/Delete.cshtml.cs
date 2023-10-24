using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models.Vimeo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NimblePros.Vimeo.VideoServices;

namespace DevBetterWeb.Web.Pages.Admin.Videos;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class DeleteModel : PageModel
{
  private readonly IMapper _mapper;
  private readonly GetVideoService _getVideoService;
  private readonly DeleteVideoService _deleteVideoService;
  private readonly IRepository<ArchiveVideo> _repository;

  public DeleteModel(IMapper mapper, GetVideoService getVideoService, DeleteVideoService deleteVideoService, IRepository<ArchiveVideo> repository)
  {
    _mapper = mapper;
    _getVideoService = getVideoService;

    _deleteVideoService = deleteVideoService;
    _repository = repository;
  }

  public VideoModel? VideoToDelete { get; set; }

  public async Task<IActionResult> OnGetAsync(string id)
  {
    if (string.IsNullOrEmpty(id))
    {
      return NotFound();
    }

		var videoId = long.Parse(id);

		var video = await _getVideoService.ExecuteAsync(videoId);
    if (video?.Data == null)
    {
			var videoDeleteRequest = new DeleteVideoRequest(videoId);
			await _deleteVideoService.ExecuteAsync(videoDeleteRequest);

      return RedirectToPage("./Index");
    }
    VideoToDelete = _mapper.Map<VideoModel>(video.Data);

    return Page();
  }

  public async Task<IActionResult> OnPostAsync(string id)
  {
    if (string.IsNullOrEmpty(id))
    {
      return NotFound();
    }

		var videoId = long.Parse(id);


		var video = await _getVideoService.ExecuteAsync(videoId);
    if (video?.Data == null)
    {
      return NotFound();
    }

		var videoDeleteRequest = new DeleteVideoRequest(videoId);
    await _deleteVideoService.ExecuteAsync(videoDeleteRequest);

    var spec = new ArchiveVideoByVideoIdSpec(videoId.ToString());
    var archiveVideo = await _repository.FirstOrDefaultAsync(spec);
    if (archiveVideo != null)
    {
      await _repository.DeleteAsync(archiveVideo);
    }

    return RedirectToPage("./Index");
  }
}
