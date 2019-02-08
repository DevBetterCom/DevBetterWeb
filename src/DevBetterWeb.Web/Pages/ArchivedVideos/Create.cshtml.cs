using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    public class CreateModel : PageModel
    {
        private readonly CleanArchitecture.Infrastructure.Data.AppDbContext _context;

        public CreateModel(CleanArchitecture.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ArchiveVideo ArchiveVideo { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ArchiveVideos.Add(ArchiveVideo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}