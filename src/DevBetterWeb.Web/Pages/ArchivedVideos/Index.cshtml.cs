using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public IList<ArchiveVideoIndexDTO> ArchiveVideoList { get; set; }

        public class ArchiveVideoIndexDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            public DateTimeOffset DateCreated { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.VideoUrl)]
            public string VideoUrl { get; set; }
        }

        public async Task OnGetAsync()
        {
            ArchiveVideoList = await _context.ArchiveVideos
                .Select(v => new ArchiveVideoIndexDTO
                {
                    Id = v.Id,
                    DateCreated = v.DateCreated,
                    Title = v.Title,
                    VideoUrl = v.VideoUrl
                })
                .ToListAsync();
        }
    }
}
