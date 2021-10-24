using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.Admin.Videos
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class EditModel : PageModel
  {
    private readonly IMapper _mapper;
    private readonly IRepository<ArchiveVideo> _repository;

    public EditModel(IMapper mapper, IRepository<ArchiveVideo> repository)
    {
      _mapper = mapper;
      _repository = repository;
    }

#nullable disable
    [BindProperty]
    public ArchiveVideoDto VideoToEdit { get; set; }
#nullable enable

    public async Task<IActionResult> OnGetAsync(string? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var spec = new ArchiveVideoByVideoIdSpec(id);
      var archiveVideo = await _repository.GetBySpecAsync(spec);
      if (archiveVideo == null) return NotFound(id);

      VideoToEdit = _mapper.Map<ArchiveVideoDto>(archiveVideo);

      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      var spec = new ArchiveVideoByVideoIdSpec(VideoToEdit.VideoId!);
      var archiveVideo = await _repository.GetBySpecAsync(spec);
      if (archiveVideo == null) return NotFound(VideoToEdit.VideoId);

      var videoToSave = _mapper.Map<ArchiveVideo>(VideoToEdit);
      if (videoToSave == null)
      {
        return NotFound();
      }

      await _repository.UpdateAsync(videoToSave);

      return RedirectToPage("./Index");
    }
  }
}
