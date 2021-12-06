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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.Books;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

public class EditModel : PageModel
{
  private readonly DevBetterWeb.Infrastructure.Data.AppDbContext _context;

  public EditModel(DevBetterWeb.Infrastructure.Data.AppDbContext context)
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

  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see https://aka.ms/RazorPagesCRUD.
  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }

    _context.Attach(Book).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!BookExists(Book!.Id))
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

  private bool BookExists(int id)
  {
    return _context.Books!.Any(e => e.Id == id);
  }
}
