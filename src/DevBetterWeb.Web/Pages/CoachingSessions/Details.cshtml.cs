using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.CoachingSessions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
public class DetailsModel : PageModel
{
	private readonly IMapper _mapper;
  private readonly IRepository<CoachingSession> _coachingSessionRepository;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IRepository<Member> _memberRepository;
  private readonly IMarkdownService _markdownService;

  public DetailsModel(
	  IMapper mapper, 
	  IRepository<CoachingSession> coachingSessionRepository,
	  UserManager<ApplicationUser> userManager,
	  IRepository<Member> memberRepository,
	  IMarkdownService markdownService)
  {
	  _mapper = mapper;
    _coachingSessionRepository = coachingSessionRepository;
    _userManager = userManager;
    _memberRepository = memberRepository;
    _markdownService = markdownService;
  }

  public CoachingSessionDto CoachingSession { get; set; } = new CoachingSessionDto();

  public async Task<IActionResult> OnGetAsync(int id)
  {
	  var currentUserName = User.Identity!.Name;
	  var applicationUser = await _userManager.FindByNameAsync(currentUserName);

	  var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
	  var member = await _memberRepository.FirstOrDefaultAsync(memberSpec);
	  if (member is null)
	  {
		  return Unauthorized();
	  }

		var spec = new CoachingSessionWithQuestionsSpec(id);
    var coachingSessionEntity = await _coachingSessionRepository.FirstOrDefaultAsync(spec);
    if (coachingSessionEntity == null)
    {
      return NotFound();
    }

    CoachingSession = _mapper.Map<CoachingSessionDto>(coachingSessionEntity);

    foreach (var question in CoachingSession.Questions)
    {
	    question.CanUpVote = coachingSessionEntity.Questions.First(x => x.Id == question.Id).MemberCanUpVote(member.Id);
	    question.QuestionText = _markdownService.RenderHTMLFromMD(question.QuestionText);
    }

    return Page();
  }
}
