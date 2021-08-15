using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models.Vimeo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.Videos
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class DeleteModel : PageModel
  {
    private readonly IMapper _mapper;
    private readonly GetVideoService _getVideoService;
    private readonly DeleteVideoService _deleteVideoService;

    public DeleteModel(IMapper mapper, GetVideoService getVideoService, DeleteVideoService deleteVideoService)
    {
      _mapper = mapper;
      _getVideoService = getVideoService;
      _getVideoService.SetToken(AuthConstants.VIMEO_TOKEN);

      _deleteVideoService = deleteVideoService;
    }

    public VideoModel? VideoToDelete { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        return NotFound();
      }

      var video = await _getVideoService.ExecuteAsync(id);
      if (video == null || video.Data == null)
      {
        await _deleteVideoService.ExecuteAsync(id);

        return RedirectToPage("./Index");
      }
      VideoToDelete = _mapper.Map<VideoModel>(video.Data);

      return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
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

      await _deleteVideoService.ExecuteAsync(id);
     
      return RedirectToPage("./Index");
    }
  }
}
