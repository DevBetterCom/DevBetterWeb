using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.CoachingSessions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
public class DetailsModel : PageModel
{
	private readonly IMapper _mapper;
  private readonly IRepository<CoachingSession> _coachingSessionRepository;

  public DetailsModel(IMapper mapper, IRepository<CoachingSession> coachingSessionRepository)
  {
	  _mapper = mapper;
    _coachingSessionRepository = coachingSessionRepository;
  }

  public CoachingSessionDto CoachingSession { get; set; } = new CoachingSessionDto();

  public async Task<IActionResult> OnGetAsync(int id, int? startTime = null)
  {
	  var spec = new CoachingSessionWithQuestionsSpec(id);
    var coachingSessionEntity = await _coachingSessionRepository.FirstOrDefaultAsync(spec);
    if (coachingSessionEntity == null)
    {
      return NotFound();
    }

    CoachingSession = _mapper.Map<CoachingSessionDto>(coachingSessionEntity);

    return Page();
  }
}
