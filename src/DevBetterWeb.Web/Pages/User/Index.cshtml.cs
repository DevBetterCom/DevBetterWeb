using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.User
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
  public class IndexModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _appDbContext;
    public readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public List<MemberLinksDTO> Members { get; set; } = new List<MemberLinksDTO>();
    public List<MemberSubscriptionPercentViewModel> PercentModels { get; set; } = new List<MemberSubscriptionPercentViewModel>();
    public bool IsAdministrator { get; set; }

    public IndexModel(UserManager<ApplicationUser> userManager,
        AppDbContext appDbContext,
        IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService)
    {
      _userManager = userManager;
      _appDbContext = appDbContext;
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
    }

    public async Task OnGet()
    {
      IsAdministrator = User.IsInRole(AuthConstants.Roles.ADMINISTRATORS);

      var usersInRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);

      // TODO: Write a LINQ join for this
      // TODO: See if we can use a specification here
      var userIds = usersInRole.Select(x => x.Id).ToList();
#nullable disable
      var members = await _appDbContext.Members
          .Include(m => m.Subscriptions)
          .AsNoTracking()
          .Where(member => userIds.Contains(member.UserId))
          .OrderBy(member => member.LastName)
          .ToListAsync();

      Members = members.Select(member => MemberLinksDTO.FromMemberEntity(member))
          .ToList();
#nullable enable

      foreach(var member in Members)
      {
        var model = new MemberSubscriptionPercentViewModel($"{_memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member.SubscribedDays)}deg");
        PercentModels.Add(model);
      }

    }

    public class MemberLinksDTO
    {
      public string? UserId { get; set; }
      public string? FullName { get; set; }
      public string? BlogUrl { get; private set; }
      public string? GitHubUrl { get; private set; }
      public string? LinkedInUrl { get; private set; }
      public string? OtherUrl { get; private set; }
      public string? TwitchUrl { get; private set; }
      public string? YouTubeUrl { get; private set; }
      public string? TwitterUrl { get; private set; }
      public string? PEUsername { get; private set; }
      public string? PEBadgeURL { get; private set; }
      public string? Address { get; private set; }
      public string? CodinGameUrl { get; private set; }
      public int SubscribedDays { get; private set; }

      public static MemberLinksDTO FromMemberEntity(Member member)
      {

        var dto = new MemberLinksDTO
        {
          FullName = member.UserFullName(),
          BlogUrl = member.BlogUrl,
          GitHubUrl = member.GitHubUrl,
          LinkedInUrl = member.LinkedInUrl,
          OtherUrl = member.OtherUrl,
          TwitchUrl = member.TwitchUrl,
          YouTubeUrl = member.YouTubeUrl,
          TwitterUrl = member.TwitterUrl,
          UserId = member.UserId,
          PEUsername = member.PEUsername,
          PEBadgeURL = $"https://projecteuler.net/profile/{member.PEUsername}.png",
          Address = member.Address,
          CodinGameUrl = member.CodinGameUrl,
          SubscribedDays = member.TotalSubscribedDays()
        };

        if (!(string.IsNullOrEmpty(dto.YouTubeUrl)) && !(dto.YouTubeUrl.Contains("?")))
        {
          dto.YouTubeUrl = dto.YouTubeUrl + "?sub_confirmation=1";
        }

        return dto;
      }
    }
  }


}
