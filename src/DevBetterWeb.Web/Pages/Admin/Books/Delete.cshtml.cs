using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.Books;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

public class DeleteModel : PageModel
{
  private readonly DevBetterWeb.Infrastructure.Data.AppDbContext _context;

  public DeleteModel(DevBetterWeb.Infrastructure.Data.AppDbContext context)
  {
    _context = context;
  }

  [BindProperty]
  public Book? Book { get; set; }

  public async Task<IActionResult> OnGetAsync(int? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    Book = await _context.Books!.AsQueryable().FirstOrDefaultAsync(m => m.Id == id);

    if (Book == null)
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

    Book = await _context.Books!.FindAsync(id);

    if (Book != null)
    {
      _context.Books.Remove(Book);
      await _context.SaveChangesAsync();
    }

    return RedirectToPage("./Index");
  }
}
