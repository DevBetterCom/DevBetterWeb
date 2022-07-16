using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.CoachingSessions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class EditModel : PageModel
{
	private readonly IMapper _mapper;
	private readonly IRepository<CoachingSession> _coachingSessionRepository;

  public EditModel(
		IMapper mapper,
	  IRepository<CoachingSession> coachingSessionRepository)
  {
	  _mapper = mapper;
	  _coachingSessionRepository = coachingSessionRepository;
  }

  [BindProperty] public CoachingSessionAddEditDto CoachingSessionModel { get; set; } = new CoachingSessionAddEditDto();

	public async Task<IActionResult> OnGetAsync(int id, int? startTime = null)
	{
		var spec = new CoachingSessionWithQuestionsSpec(id);
		var coachingSessionEntity = await _coachingSessionRepository.FirstOrDefaultAsync(spec);
		if (coachingSessionEntity == null)
		{
			return NotFound();
		}

		CoachingSessionModel = _mapper.Map<CoachingSessionAddEditDto>(coachingSessionEntity);

		return Page();
	}
}
