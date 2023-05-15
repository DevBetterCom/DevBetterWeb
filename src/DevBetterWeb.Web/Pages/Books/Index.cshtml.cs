using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Books;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class IndexModel : PageModel
{
	private readonly IBookService _bookService;

  public List<BookViewModel> Books { get; set; } = new ();

  public IndexModel(IBookService bookService)
  {
		_bookService = bookService;
  }

  public async Task OnGet()
  {
		Books = await _bookService.GetAllBooksAsync();
  }
}
