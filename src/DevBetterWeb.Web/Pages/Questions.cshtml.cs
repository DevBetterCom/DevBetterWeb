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
	private readonly IMarkdownService _markdownService;

	public QuestionsModel(IMapper mapper, IRepository<CoachingSession> coachingSessionRepository, IMarkdownService markdownService)
	{
		_mapper = mapper;
		_coachingSessionRepository = coachingSessionRepository;
		_markdownService = markdownService;
	}

  public List<CoachingSessionDto> CoachingSessions { get; set; } = new List<CoachingSessionDto>();

  public async Task OnGetAsync()
  {
	  var spec = new CoachingSessionsWithQuestionsSpec(5);
	  var coachingSessionsEntity = await _coachingSessionRepository.ListAsync(spec);
	  CoachingSessions = _mapper.Map<List<CoachingSessionDto>>(coachingSessionsEntity);
	  foreach (var coachingSession in CoachingSessions)
	  {
		  foreach (var question in coachingSession.Questions)
		  {
			  question.QuestionText = _markdownService.RenderHTMLFromMD(question.QuestionText);
		  }
	  }
	}
}
