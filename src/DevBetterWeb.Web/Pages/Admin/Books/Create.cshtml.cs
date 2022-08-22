using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.Books;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class CreateModel : PageModel
{
	public List<BookCategoryDto> BookCategories { get; set; } = new List<BookCategoryDto>();

	private readonly IRepository<Member> _memberRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Book> _bookRepository;
	private readonly IRepository<BookCategory> _bookCategoryRepository;
	private readonly IMapper _mapper;

	public CreateModel(
		IRepository<Member> memberRepository,
		UserManager<ApplicationUser> userManager,
		IRepository<Book> bookRepository,
		IRepository<BookCategory> bookCategoryRepository,
		IMapper mapper
		)
  {
		_memberRepository = memberRepository;
		_userManager = userManager;
		_bookRepository = bookRepository;
		_bookCategoryRepository = bookCategoryRepository;
		_mapper = mapper;
	}

  public async Task<IActionResult> OnGetAsync()
  {
		BookCategories = _mapper.Map<List<BookCategoryDto>>(await _bookCategoryRepository.ListAsync());

		return Page();
  }

  [BindProperty]
  public BookViewModel? Book { get; set; }
  public async Task<IActionResult> OnPostAsync()
  {
		BookCategories = _mapper.Map<List<BookCategoryDto>>(await _bookCategoryRepository.ListAsync());

		if (!ModelState.IsValid)
    {
      return Page();
    }
    if (Book == null) return Page();

		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName);

		var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(spec);

		var bookEntity = new Book
    {
      Author = Book.Author,
      Details = Book.Details,
      PurchaseUrl = Book.PurchaseUrl,
      Title = Book.Title,
			BookCategoryId = Book.BookCategoryId,
			MemberWhoUploadId = member!.Id
    };

    var bookAddedEvent = new NewBookCreatedEvent(bookEntity);
    bookEntity.Events.Add(bookAddedEvent);

    await _bookRepository.AddAsync(bookEntity);

    return RedirectToPage("./Index");
  }
}
