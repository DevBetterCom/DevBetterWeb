using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User
{
  [Authorize]
  public class DetailsModel : PageModel
  {
#pragma warning disable CS0436 // Type conflicts with imported type
    public UserDetailsViewModel? UserDetailsViewModel { get; set; }
#pragma warning restore CS0436 // Type conflicts with imported type
    private readonly IRepository<Member> _memberRepository;
    private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;
    private readonly IAppLogger<DetailsModel> _logger;

    public MemberSubscriptionPercentBarViewModel PercentModel { get; set; } = new MemberSubscriptionPercentBarViewModel(0);

    public DetailsModel(IRepository<Member> memberRepository,
      IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService,
      IAppLogger<DetailsModel> logger)
    {
      _memberRepository = memberRepository;
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
      _logger = logger;
    }

    public async Task<ActionResult> OnGet(string userId)
    {

      var spec = new MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec(userId);
      var member = await _memberRepository.GetBySpecAsync(spec);

      if (member == null)
      {
        _logger.LogWarning($"No member found for userId {userId}");
        return NotFound();
      }

#pragma warning disable CS0436 // Type conflicts with imported type
      UserDetailsViewModel = new UserDetailsViewModel(member!);
#pragma warning restore CS0436 // Type conflicts with imported type

      int percentageNum = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member!);

      PercentModel.Percentage = percentageNum;

      return Page();
    }
  }
}
