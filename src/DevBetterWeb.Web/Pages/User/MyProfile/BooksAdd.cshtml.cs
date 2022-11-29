using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User;

[Authorize(Roles = AuthConstants.Roles.ALUMNI)]
public class MyProfileBooksAddModel : PageModel
{
#nullable disable
  [BindProperty]
  public UserBooksAddModel UserBooksAddModel { get; set; }
  public List<BookCategoryDto> BookCategories { get; set; } = new List<BookCategoryDto>();
  public Book AddedBook { get; set; }

#nullable enable

  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMemberRegistrationService _memberRegistrationService;
  private readonly IRepository<Member> _memberRepository;
  private readonly IRepository<Book> _bookRepository;
	private readonly IRepository<BookCategory> _bookCategoryRepository;
	private readonly AppDbContext _appDbContext;
	private readonly IMapper _mapper;

	public MyProfileBooksAddModel(UserManager<ApplicationUser> userManager,
      IMemberRegistrationService memberRegistrationService,
      IRepository<Member> memberRepository,
      IRepository<Book> bookRepository,
      IRepository<BookCategory> bookCategoryRepository,
      AppDbContext appDbContext,
			IMapper mapper)
  {
    _userManager = userManager;
    _memberRegistrationService = memberRegistrationService;
    _memberRepository = memberRepository;
    _bookRepository = bookRepository;
		_bookCategoryRepository = bookCategoryRepository;
		_appDbContext = appDbContext;
		_mapper = mapper;
	}

  public async Task OnGetAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser!.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);

    if (member == null)
    {
      member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
    }

		BookCategories = _mapper.Map<List<BookCategoryDto>>(await _bookCategoryRepository.ListAsync());

		UserBooksAddModel = new UserBooksAddModel(member);
  }

  public async Task<ActionResult> OnPostAdd()
  {
		BookCategories = _mapper.Map<List<BookCategoryDto>>(await _bookCategoryRepository.ListAsync());

		if (!ModelState.IsValid) return Page();

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser!.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);
    if (member is null) throw new MemberNotFoundException(applicationUser.Id);

		if (UserBooksAddModel.AddedBook == null)
		{
			return RedirectToPage();
		}

		UserBooksAddModel.AddedBook!.MemberWhoUploadId = member.Id;
		var bookToAdd = _mapper.Map<Book>(UserBooksAddModel.AddedBook);

    member.AddBookForAdd(bookToAdd);    
    await _memberRepository.UpdateAsync(member);

    return RedirectToPage();
  }
}
