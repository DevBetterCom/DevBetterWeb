using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard
{
  [Authorize]
  public class BookDetailsModel : PageModel
  {
    public BookDetailsViewModel? BookDetailsViewModel { get; set; }
    private readonly IRepository<Book> _bookRepository;

    public BookDetailsModel(IRepository<Book> bookRepository)
    {
      _bookRepository = bookRepository;
    }

    public async Task<IActionResult> OnGet(string bookId)
    {
      var spec = new BookByIdWithMembersSpec(int.Parse(bookId));
      var book = await _bookRepository.GetBySpecAsync(spec);

      if (book == null) return NotFound(bookId);

      BookDetailsViewModel = new BookDetailsViewModel(book!);
      return Page();
    }
  }
}
