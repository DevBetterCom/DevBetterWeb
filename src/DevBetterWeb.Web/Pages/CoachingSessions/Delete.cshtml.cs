using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.CoachingSessions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class DeleteModel : PageModel
{
	private readonly IMapper _mapper;
	private readonly IRepository<CoachingSession> _coachingSessionRepository;

  public DeleteModel(IMapper mapper, IRepository<CoachingSession> coachingSessionRepository)
  {
	  _mapper = mapper;
	  _coachingSessionRepository = coachingSessionRepository;
  }

  protected class DeleteVideoModel
  {
    public int Id { get; set; }
  }

  public CoachingSessionDto? CoachingSessionToDelete { get; set; }

  public async Task<IActionResult> OnGetAsync(int id)
  {
    var coachingSession = await _coachingSessionRepository.GetByIdAsync(id);

    if (coachingSession == null)
    {
      return NotFound();
    }

    CoachingSessionToDelete = _mapper.Map<CoachingSessionDto>(coachingSession);
    return Page();
  }

  public async Task<IActionResult> OnPostAsync(int id)
  {
	  var coachingSession = await _coachingSessionRepository.GetByIdAsync(id);

		if (coachingSession != null)
    {
			await _coachingSessionRepository.DeleteAsync(coachingSession);
		}

    return RedirectToPage("./Index");
  }
}
