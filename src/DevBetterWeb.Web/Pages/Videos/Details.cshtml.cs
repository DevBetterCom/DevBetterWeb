using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Videos
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  public class DetailsModel : PageModel
  {
    [BindProperty]
    public OEmbedViewModel? OEmbedViewModel { get; set; }

    private readonly GetOEmbedVideoService _getOEmbedVideoService;
    private readonly GetVideoService _getVideoService;
    private readonly IRepository<ArchiveVideo> _repository;
    private readonly IMarkdownService _markdownService;

    public DetailsModel(IMarkdownService markdownService, GetOEmbedVideoService getOEmbedVideoService, GetVideoService getVideoService, IRepository<ArchiveVideo> repository)
    {
      _markdownService = markdownService;
      _getVideoService = getVideoService;
      _repository = repository;
      _getOEmbedVideoService = getOEmbedVideoService;
    }

    public async Task<IActionResult> OnGet(string videoId, string? startTime=null)
    {
      var video = await _getVideoService.ExecuteAsync(videoId);
      if (video?.Data == null) return NotFound(videoId);

      var oEmbed = await _getOEmbedVideoService.ExecuteAsync(video.Data.Link);
      if (oEmbed?.Data == null) return NotFound(videoId);

      var spec = new ArchiveVideoByVideoIdSpec(videoId);
      var archiveVideo = await _repository.GetBySpecAsync(spec);
      if (archiveVideo == null) return NotFound(videoId);

      OEmbedViewModel = new OEmbedViewModel(oEmbed.Data);
      OEmbedViewModel.Name = archiveVideo.Title;
      OEmbedViewModel.Password = video.Data.Password;
      OEmbedViewModel.Description = _markdownService.RenderHTMLFromMD(archiveVideo.Description);
      OEmbedViewModel
        .AddStartTime(startTime)
        .BuildHtml(video.Data.Link);

      return Page();
    }
  }
}
