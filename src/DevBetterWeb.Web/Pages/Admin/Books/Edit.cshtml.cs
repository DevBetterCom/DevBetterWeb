using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.Books;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

public class EditModel : PageModel
{
	public List<BookCategoryDto> BookCategories { get; set; } = new List<BookCategoryDto>();

	private readonly IRepository<Book> _bookRepository;
	private readonly IRepository<BookCategory> _bookCategoryRepository;
	private readonly IMapper _mapper;
	private int? MemberWhoUploadId;

	public EditModel(
		IRepository<Book> bookRepository,
		IRepository<BookCategory> bookCategoryRepository,
		IMapper mapper)
  {
		_bookRepository = bookRepository;
		_bookCategoryRepository = bookCategoryRepository;
		_mapper = mapper;
	}

  [BindProperty]
  public Book? Book { get; set; }

  public async Task<IActionResult> OnGetAsync(int? id)
  {
		BookCategories = _mapper.Map<List<BookCategoryDto>>(await _bookCategoryRepository.ListAsync());

		if (id == null)
    {
      return NotFound();
    }

		var spec = new BookByIdWithMembersSpec(id.Value);
		Book = await _bookRepository.FirstOrDefaultAsync(spec);	
		if (Book == null)
    {
      return NotFound();
    }

		MemberWhoUploadId = Book!.MemberWhoUploadId;
		return Page();
  }

  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see https://aka.ms/RazorPagesCRUD.
  public async Task<IActionResult> OnPostAsync()
  {
		BookCategories = _mapper.Map<List<BookCategoryDto>>(await _bookCategoryRepository.ListAsync());

		if (!ModelState.IsValid)
    {
      return Page();
    }

    if (Book == null) return NotFound();

		var bookEntity = new Book
		{
			Author = Book.Author,
			Details = Book.Details,
			PurchaseUrl = Book.PurchaseUrl,
			Title = Book.Title,
			BookCategoryId = Book.BookCategoryId,
			MemberWhoUploadId = MemberWhoUploadId
		};
		await _bookRepository.UpdateAsync(bookEntity);

		try
    {
      await _bookRepository.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!await BookExistsAsync(Book!.Id))
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

  private async Task<bool> BookExistsAsync(int id)
  {
		var spec = new BookByIdWithMembersSpec(id);

		return await _bookRepository.AnyAsync(spec);
  }
}
