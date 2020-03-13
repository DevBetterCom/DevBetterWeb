using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
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

        public List<KeyValuePair<string, string>> UserIdsAndNames { get; set; } = new List<KeyValuePair<string, string>>();

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
                .Where(member => userIds.Contains(member.UserId)).ToListAsync();

            UserIdsAndNames = members
                .OrderBy(member => member.LastName)
                .Select(member => new KeyValuePair<string, string>(member.UserId, member.UserFullName()))                
                .ToList();
#nullable enable

        }
    }

   
}