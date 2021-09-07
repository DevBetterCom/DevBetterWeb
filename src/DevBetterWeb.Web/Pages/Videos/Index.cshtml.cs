using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models.Vimeo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.Videos
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  public class IndexModel : PageModel
  {
    private readonly IMapper _mapper;
    private readonly GetAllVideosService _getAllVideosService;

    [BindProperty]
    public IList<VideoModel> VideoList { get; set; } = new List<VideoModel>();
    [BindProperty]
    public int PagesCount { get; set; } = 0;
    [BindProperty]
    public int SelectedPage { get; set; } = 1;


    public IndexModel(IMapper mapper, GetAllVideosService getAllVideosService)
    {
      _mapper = mapper;
      _getAllVideosService = getAllVideosService;
    }

    public async Task<IActionResult> OnGetAsync(int? pageIndex = 1, int? pageSize=10)
    {
      SelectedPage = pageIndex.Value;
      var request = new GetAllVideosRequest("me", pageIndex, pageSize);
      var response = await _getAllVideosService
        .ExecuteAsync(request);

      if (response.Data?.Data != null)
      {
        VideoList = _mapper.Map<List<VideoModel>>(response.Data.Data);
        PagesCount = response.Data.Total/pageSize.Value;
      }
      return Page();
    }
  }
}
