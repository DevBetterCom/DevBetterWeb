using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.Books
{
  public class CreateModel : PageModel
  {
    private readonly IRepository _repository;

    public CreateModel(IRepository repository)
    {
      _repository = repository;
    }

    public IActionResult OnGet()
    {
      return Page();
    }

    [BindProperty]
    public BookViewModel? Book { get; set; }
    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }
      if (Book == null) return Page();

      var bookEntity = new Book
      {
        Author = Book.Author,
        Details = Book.Details,
        PurchaseUrl = Book.PurchaseUrl,
        Title = Book.Title
      };

      var bookAddedEvent = new NewBookCreatedEvent(bookEntity);
      bookEntity.Events.Add(bookAddedEvent);

      await _repository.AddAsync(bookEntity);

      return RedirectToPage("./Index");
    }
  }
}
