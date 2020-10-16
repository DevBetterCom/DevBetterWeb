using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;

namespace DevBetterWeb.Web.Pages.Admin.Books
{
    public class CreateModel : PageModel
    {
        private readonly DevBetterWeb.Infrastructure.Data.AppDbContext _context;

        public CreateModel(DevBetterWeb.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Book? Book { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Books!.Add(Book!);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
