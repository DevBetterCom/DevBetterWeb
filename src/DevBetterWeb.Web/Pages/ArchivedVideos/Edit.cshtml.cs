using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    public class EditModel : PageModel
    {
        private readonly CleanArchitecture.Infrastructure.Data.AppDbContext _context;

        public EditModel(CleanArchitecture.Infrastructure.Data.AppDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ArchiveVideo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArchiveVideoExists(ArchiveVideo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArchiveVideoExists(int id)
        {
            return _context.ArchiveVideos.Any(e => e.Id == id);
        }
    }
}
