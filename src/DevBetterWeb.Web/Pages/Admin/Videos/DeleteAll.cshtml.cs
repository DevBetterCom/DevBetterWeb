using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NimblePros.ApiClient.Interfaces;
using NimblePros.Vimeo.Models;
using NimblePros.Vimeo.Services.VideoServices;
using NimblePros.Vimeo.VideoServices;

namespace DevBetterWeb.Web.Pages.Admin.Videos;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class DeleteAllModel : PageModel
{
  private readonly GetVideosUserAppearsService _getPagedVideosService;
  private readonly GetVideoService _getVideoService;
  private readonly DeleteVideoService _deleteVideoService;
  private readonly IRepository<ArchiveVideo> _repository;

  public DeleteAllModel(GetVideosUserAppearsService getPagedVideosService, GetVideoService getVideoService, DeleteVideoService deleteVideoService, IRepository<ArchiveVideo> repository)
  {
    _getPagedVideosService = getPagedVideosService;
    _getVideoService = getVideoService;

    _deleteVideoService = deleteVideoService;
    _repository = repository;
  }

  public IActionResult OnGet()
  {
    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    await DeleteAllVimeoVideosAsync();
    await DeleteAllArchiveVideosAsync();

    return RedirectToPage("./Index");
  }

  private async Task DeleteAllVimeoVideosAsync()
  {
    IApiResponse<DataPaged<Video>> allVideos;
    var videosToDelete = new List<Video>();
    var pageNumber = 1;
    do
    {
      var getAllRequest = new GetVideosUserAppearsRequest();
			getAllRequest.Page = pageNumber;
			allVideos = await _getPagedVideosService.ExecuteAsync(getAllRequest);
      if (allVideos != null && allVideos.Data != null)
      {
        videosToDelete.AddRange(allVideos.Data.Data);
      }
      pageNumber++;
    } while (allVideos != null && allVideos.Data != null);

    foreach (var video in videosToDelete)
    {
			var deleteVideoRequest = new DeleteVideoRequest(video.Id);
      await _deleteVideoService.ExecuteAsync(deleteVideoRequest);
    }
  }

  private async Task DeleteAllArchiveVideosAsync()
  {
    var archiveVideos = await _repository.ListAsync();
    if (archiveVideos != null)
    {
      await _repository.DeleteRangeAsync(archiveVideos);
    }
  }
}
