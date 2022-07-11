using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class MyProfileBooksModel : PageModel
{
#nullable disable
  [BindProperty]
  public UserBooksUpdateModel UserBooksUpdateModel { get; set; }
  public List<Book> Books { get; set; } = new List<Book>();
  public Book AddedBook { get; set; }
  public Book RemovedBook { get; set; }

#nullable enable

  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMemberRegistrationService _memberRegistrationService;
  private readonly IRepository<Member> _memberRepository;
  private readonly IRepository<Book> _bookRepository;
  private readonly AppDbContext _appDbContext;

  public MyProfileBooksModel(UserManager<ApplicationUser> userManager,
      IMemberRegistrationService memberRegistrationService,
      IRepository<Member> memberRepository,
      IRepository<Book> bookRepository,
      AppDbContext appDbContext)
  {
    _userManager = userManager;
    _memberRegistrationService = memberRegistrationService;
    _memberRepository = memberRepository;
    _bookRepository = bookRepository;
    _appDbContext = appDbContext;
  }

  public async Task OnGetAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);

    if (member == null)
    {
      member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
    }

    Books = await _bookRepository.ListAsync();

    UserBooksUpdateModel = new UserBooksUpdateModel(member);
  }

  public async Task<ActionResult> OnPostAdd()
  {
    if (!ModelState.IsValid) return Page();
    // TODO: consider only getting the user alias not the whole URL for social media links
    // TODO: assess risk of XSS attacks and how to mitigate

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);
    if (member is null) throw new MemberNotFoundException(applicationUser.Id);

    if (UserBooksUpdateModel.AddedBook.HasValue)
    {
      AddedBook = await _bookRepository.GetByIdAsync(UserBooksUpdateModel.AddedBook.Value);
      if (AddedBook is null) throw new BookNotFoundException(UserBooksUpdateModel.AddedBook.Value);
      member.AddBookRead(AddedBook);
    }

    await _memberRepository.UpdateAsync(member);

    return RedirectToPage();
  }
  public async Task<ActionResult> OnPostRemove()
  {
    if (!ModelState.IsValid) return Page();
    // TODO: consider only getting the user alias not the whole URL for social media links
    // TODO: assess risk of XSS attacks and how to mitigate

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);
    if (member is null) throw new MemberNotFoundException(applicationUser.Id);

    if (UserBooksUpdateModel.RemovedBook.HasValue)
    {
      RemovedBook = await _bookRepository.GetByIdAsync(UserBooksUpdateModel.RemovedBook.Value);
      if (RemovedBook is null) throw new BookNotFoundException(UserBooksUpdateModel.RemovedBook.Value);
      member.RemoveBookRead(RemovedBook);
    }

    await _memberRepository.UpdateAsync(member);

    return RedirectToPage();
  }
}
