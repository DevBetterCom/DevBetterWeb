using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.CoachingSessions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
public class IndexModel : PageModel
{
	private readonly IMapper _mapper;
	private readonly IRepository<CoachingSession> _coachingSessionRepository;

	public IndexModel(IMapper mapper, IRepository<CoachingSession> coachingSessionRepository)
	{
		_mapper = mapper;
		_coachingSessionRepository = coachingSessionRepository;
	}

  public IList<CoachingSessionDto> CoachingSessions { get; set; } = new List<CoachingSessionDto>();

  public async Task OnGetAsync()
  {
	  var coachingSessionEntity = await _coachingSessionRepository.ListAsync();
    CoachingSessions = _mapper.Map<List<CoachingSessionDto>>(coachingSessionEntity);
  }
}
