using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models.Vimeo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.Videos
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class EditModel : PageModel
  {
    private readonly IMapper _mapper;
    private readonly GetVideoService _getVideoService;
    private readonly UpdateVideoDetailsService _updateVideoDetailsService;

    public EditModel(IMapper mapper, GetVideoService getVideoService, UpdateVideoDetailsService updateVideoDetailsService)
    {
      _mapper = mapper;
      _getVideoService = getVideoService;
      _getVideoService.SetToken(AuthConstants.VIMEO_TOKEN);
      _updateVideoDetailsService = updateVideoDetailsService;
    }

#nullable disable
    [BindProperty]
    public VideoModel VideoToEdit { get; set; }
#nullable enable

    public async Task<IActionResult> OnGetAsync(string? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var video = await _getVideoService.ExecuteAsync(id);
      if (video == null || video.Data == null)
      {
        return NotFound();
      }

      VideoToEdit = _mapper.Map<VideoModel>(video.Data);

      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      var currentVideo = await _getVideoService.ExecuteAsync(VideoToEdit.Id);
      if (currentVideo == null || currentVideo.Data == null)
      {
        return NotFound();
      }

      var videoToSave = _mapper.Map<Video>(VideoToEdit);

      var request = new UpdateVideoDetailsRequest(long.Parse(VideoToEdit.Id), videoToSave);
      await _updateVideoDetailsService.ExecuteAsync(request);

      return RedirectToPage("./Index");
    }
  }
}
