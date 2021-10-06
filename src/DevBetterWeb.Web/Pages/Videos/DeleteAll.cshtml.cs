using Ardalis.ApiCaller;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.Videos
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class DeleteAllModel : PageModel
  {
    private readonly GetAllVideosService _getAllVideosService;
    private readonly GetVideoService _getVideoService;
    private readonly DeleteVideoService _deleteVideoService;
    private readonly IRepository<ArchiveVideo> _repository;

    public DeleteAllModel(GetAllVideosService getAllVideosService, GetVideoService getVideoService, DeleteVideoService deleteVideoService, IRepository<ArchiveVideo> repository)
    {
      _getAllVideosService = getAllVideosService;
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
      await deleteAllVimeoVideosAsync();
      await deleteAllArchiveVideosAsync();

      return RedirectToPage("./Index");
    }

    private async Task deleteAllVimeoVideosAsync()
    {
      HttpResponse<DataPaged<Video>> allVideos;
      var videosToDelete = new List<Video>();
      var pageNumber = 1;
      do
      {
        var getAllRequest = new GetAllVideosRequest(ServiceConstants.ME, pageNumber);
        allVideos = await _getAllVideosService.ExecuteAsync(getAllRequest);
        if (allVideos != null && allVideos.Data != null)
        {
          videosToDelete.AddRange(allVideos.Data.Data);
        }
        pageNumber++;
      } while (allVideos != null && allVideos.Data != null);

      foreach (var video in videosToDelete)
      {
        await _deleteVideoService.ExecuteAsync(video.Id);
      }
    }

    private async Task deleteAllArchiveVideosAsync()
    {
      var archiveVideos = await _repository.ListAsync();
      if (archiveVideos != null)
      {
        await _repository.DeleteRangeAsync(archiveVideos);
      }
    }
  }
}
