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
  private readonly IRepository<BookCategory> _bookCategoryRepository;
  private readonly IMapper _mapper;

  public List<BookCategoryDto> BookCategories { get; set; } = new List<BookCategoryDto>();

  public IndexModel(UserManager<ApplicationUser> userManager,
      IRepository<Member> memberRepository,
      IRepository<BookCategory> bookCategoryRepository,
      IMapper mapper)
  {
    _userManager = userManager;
    _memberRepository = memberRepository;
    _bookCategoryRepository = bookCategoryRepository;
    _mapper = mapper;
  }

  public async Task OnGet()
  {
		var alumniMembers = await GetAlumniMembersAsync();
		var alumniMembersIds = alumniMembers.Select(x => x.Id).ToList();
		await SetBookCategoriesAsync(alumniMembersIds);
  }

  private async Task SetBookCategoriesAsync(List<int> alumniMembersIds)
  {
		var spec = new BookCategoriesSpec();
		var bookCategoriesEntity = await _bookCategoryRepository.ListAsync(spec);
		BookCategory.CalcAndSetCategoriesBooksRank(bookCategoriesEntity);
		BookCategory.CalcAndSetMemberCategoriesMembersRank(bookCategoriesEntity);
		BookCategory.AddMembersRole(bookCategoriesEntity, alumniMembersIds);
		BookCategories = _mapper.Map<List<BookCategoryDto>>(bookCategoriesEntity);
		UpdateMembersReadCount();
		UpdateMembersReadRank();
		OderByRankForMembersAndBooks();
	}

  private void UpdateMembersReadCount()
  {
	  foreach (var category in BookCategories)
	  {
		  foreach (var member in category.Members)
		  {
			  member.BooksReadCount = member.BooksRead!.Count(x => x.BookCategoryId == category.Id);
		  }
		  foreach (var member in category.Alumnus)
		  {
			  member.BooksReadCount = member.BooksRead!.Count(x => x.BookCategoryId == category.Id);
		  }
		}
  }

  private void UpdateMembersReadRank()
  {
	  foreach (var category in BookCategories)
	  {
		  CalcMemberRank(category.Id, category.Members);
		  CalcMemberRank(category.Id, category.Alumnus);
	  }
  }

  private void CalcMemberRank(int? bookCategoryId, List<MemberForBookDto> members)
  {
	  var memberRanks = RankingService<int>.Rank(members.Select(m => m.BooksRead!.Count(b => bookCategoryId != null && b.BookCategoryId == bookCategoryId)));
	  members.ForEach(m => m.BooksRank = memberRanks[m.BooksRead!.Count(b => bookCategoryId != null && b.BookCategoryId == bookCategoryId)]);
	}

	private void OderByRankForMembersAndBooks()
	{
		foreach (var bookCategory in BookCategories)
		{
			bookCategory.Members = bookCategory.Members.OrderBy(x => x.BooksRank).ToList();
			bookCategory.Books = bookCategory.Books!.OrderBy(x => x.Rank).ToList();
		}
	}

	private async Task<List<Member>> GetAlumniMembersAsync()
  {
	  var usersInAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);
	  var alumniUserIds = usersInAlumniRole.Select(x => x.Id).ToList();

		var alumniSpec = new MembersHavingUserIdsWithBooksSpec(alumniUserIds);
	  var alumniMembers = await _memberRepository.ListAsync(alumniSpec);

	  return alumniMembers;
  }
}
