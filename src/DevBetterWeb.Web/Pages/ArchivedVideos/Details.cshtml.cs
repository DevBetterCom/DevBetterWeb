using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
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

            ArchiveVideo = await _context.ArchiveVideos.FirstOrDefaultAsync(m => m.Id == id);

            if (ArchiveVideo == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
