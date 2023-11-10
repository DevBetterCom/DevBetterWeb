using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.Books;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

public class DetailsModel : PageModel
{
  private readonly DevBetterWeb.Infrastructure.Data.AppDbContext _context;

  public DetailsModel(DevBetterWeb.Infrastructure.Data.AppDbContext context)
  {
    _context = context;
  }

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
}
