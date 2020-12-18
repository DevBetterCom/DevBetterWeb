using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.Books
{
  public class CreateModel : PageModel
  {
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
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
      if (Book == null) return Page();

      var bookAddedEvent = new NewBookCreatedEvent(Book);
      Book.Events.Add(bookAddedEvent);

      _context.Books!.Add(Book!);
      await _context.SaveChangesAsync();

      return RedirectToPage("./Index");
    }
  }
}
