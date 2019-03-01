using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = Constants.Roles.ADMINISTRATORS_MEMBERS)]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IList<ArchiveVideo> ArchiveVideo { get; set; }

        public async Task OnGetAsync()
        {
            ArchiveVideo = await _context.ArchiveVideos.ToListAsync();

            // add specific user to admin role if they're not already
            if (User.Identity.Name == "admin@test.com")
            {
                if (!User.IsInRole(Constants.Roles.ADMINISTRATORS))
                {
                    var user = await _userManager.GetUserAsync(User);
                    await _userManager.AddToRoleAsync(user, Constants.Roles.ADMINISTRATORS);
                }
            }
            if (User.Identity.Name == "member@test.com")
            {
                if (!User.IsInRole(Constants.Roles.MEMBERS))
                {
                    var user = await _userManager.GetUserAsync(User);
                    await _userManager.AddToRoleAsync(user, Constants.Roles.MEMBERS);
                }
            }
        }
    }
}
