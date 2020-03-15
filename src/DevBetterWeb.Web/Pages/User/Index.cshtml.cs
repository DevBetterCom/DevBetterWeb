using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
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

        public List<MemberLinksDTO> Members { get; set; } = new List<MemberLinksDTO>();

        public IndexModel(UserManager<ApplicationUser> userManager, 
            AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }
         
        public async Task OnGet()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);

            // TODO: Write a LINQ join for this
            // TODO: See if we can use a specification here
            var userIds = usersInRole.Select(x => x.Id).ToList();
#nullable disable
            var members = await _appDbContext.Members.AsNoTracking()
                .Where(member => userIds.Contains(member.UserId))
                .OrderBy(member => member.LastName)
                .ToListAsync();

            Members = members.Select(member => MemberLinksDTO.FromMemberEntity(member))
                .ToList();
#nullable enable

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
            public string? TwitterUrl { get; private set; }

            public static MemberLinksDTO FromMemberEntity(Member member)
            {
                return new MemberLinksDTO
                {
                    FullName = member.UserFullName(),
                    BlogUrl = member.BlogUrl,
                    GitHubUrl = member.GitHubUrl,
                    LinkedInUrl = member.LinkedInUrl,
                    OtherUrl = member.OtherUrl,
                    TwitchUrl = member.TwitchUrl,
                    TwitterUrl = member.TwitterUrl,
                    UserId = member.UserId
                };
            }
        }
    }

   
}