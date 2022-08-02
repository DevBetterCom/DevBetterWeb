using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages;

public class QuestionsModel : PageModel
{
	private readonly IMapper _mapper;
	private readonly IRepository<CoachingSession> _coachingSessionRepository;

	public QuestionsModel(IMapper mapper, IRepository<CoachingSession> coachingSessionRepository)
	{
		_mapper = mapper;
		_coachingSessionRepository = coachingSessionRepository;
	}

  public List<CoachingSessionDto> CoachingSessions { get; set; } = new List<CoachingSessionDto>();

  public async Task OnGetAsync()
  {
	  var spec = new CoachingSessionsWithQuestionsSpec();
	  var coachingSessionsEntity = await _coachingSessionRepository.ListAsync(spec);
	  CoachingSessions = _mapper.Map<List<CoachingSessionDto>>(coachingSessionsEntity);
  }
}
