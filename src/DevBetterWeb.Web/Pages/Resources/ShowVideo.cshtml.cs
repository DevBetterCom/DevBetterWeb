using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;

namespace DevBetterWeb.Web.Pages.Resources
{
  [Authorize]
  public class ShowVideoModel : PageModel
  {
    [BindProperty]
    public OEmbedViewModel? OEmbedViewModel { get; set; }

    private readonly GetOEmbedVideoService _getOEmbedVideoService;
    private readonly GetVideoService _getVideoService;

    public ShowVideoModel(GetOEmbedVideoService getOEmbedVideoService, GetVideoService getVideoService)
    {
      _getOEmbedVideoService = getOEmbedVideoService;
      _getOEmbedVideoService.SetToken(AuthConstants.VIMEO_TOKEN);

      _getVideoService = getVideoService;
    }

    public async Task<IActionResult> OnGet(string videoId, string startTime=null)
    {
      var video = await _getVideoService.ExecuteAsync(videoId);
      if (video?.Data == null) return NotFound(videoId);

      var oEmbed = await _getOEmbedVideoService.ExecuteAsync(video.Data.Link);
      if (oEmbed?.Data == null) return NotFound(videoId);

      OEmbedViewModel = new OEmbedViewModel(oEmbed.Data);
      OEmbedViewModel.Name = video.Data.Name;
      OEmbedViewModel.Password = video.Data.Password;
      OEmbedViewModel.Description = video.Data.Description;
      OEmbedViewModel.UpdateHtmlByStartTime(video.Data.Link, startTime);

      return Page();
    }
  }
}
