using CleanArchitecture.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = Constants.Roles.ADMINISTRATORS)]
    public class DeleteModel : PageModel
    {
        private readonly CleanArchitecture.Infrastructure.Data.AppDbContext _context;

        public DeleteModel(CleanArchitecture.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ArchiveVideo ArchiveVideo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ArchiveVideo = await _context.ArchiveVideos.FirstOrDefaultAsync(m => m.Id == id);

            if (ArchiveVideo == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ArchiveVideo = await _context.ArchiveVideos.FindAsync(id);

            if (ArchiveVideo != null)
            {
                _context.ArchiveVideos.Remove(ArchiveVideo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
