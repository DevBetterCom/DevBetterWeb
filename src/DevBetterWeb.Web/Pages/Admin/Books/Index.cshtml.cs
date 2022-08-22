using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.Books;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

public class IndexModel : PageModel
{
	private readonly IMapper _mapper;
	private readonly IRepository<Book> _bookRepository;

	public IndexModel(IMapper mapper, IRepository<Book> bookRepository)
  {
		_mapper = mapper;
		_bookRepository = bookRepository;
	}

  public IList<BookViewModel>? Books { get; set; }

  public async Task OnGetAsync()
  {
		var spec = new BooksWithMemberUploadedSpec();
		var books = await _bookRepository.ListAsync(spec);
		Books = _mapper.Map<List<BookViewModel>>(books);
  }
}
