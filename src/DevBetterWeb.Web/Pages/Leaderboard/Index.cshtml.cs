using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class IndexModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IRepository<Member> _memberRepository;
  private readonly IRepository<Book> _bookRepository;
  private readonly IRepository<BookCategory> _bookCategoryRepository;
  private readonly IMapper _mapper;
  private readonly RankingService<int> _rankingService = new RankingService<int>();

  public List<MemberForBookDto> Members { get; set; } = new List<MemberForBookDto>();
  public List<MemberForBookDto> Alumni { get; set; } = new List<MemberForBookDto>();
  public List<BookDto> Books { get; set; } = new List<BookDto>();
  public List<BookCategoryDto> BookCategories { get; set; } = new List<BookCategoryDto>();

  public IndexModel(UserManager<ApplicationUser> userManager,
      IRepository<Member> memberRepository,
      IRepository<Book> bookRepository,
      IRepository<BookCategory> bookCategoryRepository,
      IMapper mapper)
  {
    _userManager = userManager;
    _memberRepository = memberRepository;
    _bookRepository = bookRepository;
    _bookCategoryRepository = bookCategoryRepository;
    _mapper = mapper;
  }

  public async Task OnGet()
  {
		var alumniMembers = await SetAlumniMembersAsync();
		await SetMembersAsync(alumniMembers.Select(x => x.Id).ToList());
		await SetBooksAsync();
		await SetBookCategoriesAsync();
  }

  private async Task SetBookCategoriesAsync()
  {
		var spec = new BookCategoriesSpec();
		var bookCategoriesEntity = await _bookCategoryRepository.ListAsync(spec);
		BookCategory.CalcAndSetCategoriesBooksRank(_rankingService, bookCategoriesEntity);
		BookCategory.CalcAndSetMemberCategoriesMembersRank(_rankingService, bookCategoriesEntity);
		BookCategories = _mapper.Map<List<BookCategoryDto>>(bookCategoriesEntity);
	}

	private async Task SetBooksAsync()
  {
	  var bookSpec = new BooksByMemberReadCountWithMembersWhoHaveReadSpec();
	  var booksEntity = await _bookRepository.ListAsync(bookSpec);
	  Book.CalcAndSetRank(_rankingService, booksEntity);
	  Books = _mapper.Map<List<BookDto>>(booksEntity);
	}

  private async Task<List<Member>> SetMembersAsync(List<int> excludedAlumniMembersIds)
  {
	  var usersInMemberRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
	  var memberUserIds = usersInMemberRole.Select(x => x.Id).ToList();

	  var memberSpec = new MembersHavingUserIdsWithBooksSpec(memberUserIds, excludedAlumniMembersIds);
	  var members = await _memberRepository.ListAsync(memberSpec);
	  Member.CalcAndSetBooksRank(_rankingService, members);
	  Members = _mapper.Map<List<MemberForBookDto>>(members);

	  return members;
  }

  private async Task<List<Member>> SetAlumniMembersAsync()
  {
	  var usersInAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);
	  var alumniUserIds = usersInAlumniRole.Select(x => x.Id).ToList();

		var alumniSpec = new MembersHavingUserIdsWithBooksSpec(alumniUserIds);
	  var alumniMembers = await _memberRepository.ListAsync(alumniSpec);
	  Member.CalcAndSetBooksRank(_rankingService, alumniMembers);
	  Alumni = _mapper.Map<List<MemberForBookDto>>(alumniMembers);

	  return alumniMembers;
  }
}
