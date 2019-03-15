using CleanArchitecture.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = Constants.Roles.ADMINISTRATORS_MEMBERS)]
    public class DetailsModel : PageModel
    {
        private readonly CleanArchitecture.Infrastructure.Data.AppDbContext _context;

        public DetailsModel(CleanArchitecture.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        public ArchiveVideo ArchiveVideo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ArchiveVideo = await _context.ArchiveVideos
                .Include(v => v.Questions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ArchiveVideo == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
