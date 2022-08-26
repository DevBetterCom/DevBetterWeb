using System;
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
  private readonly RankingService<int> _rankingService = new RankingService<int>();

  public List<MemberForBookDto> Alumni { get; set; } = new List<MemberForBookDto>();
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
		var alumniMembers = await SetAlumniMembersAsync();
		var excludedAlumniMembersIds = alumniMembers.Select(x => x.Id).ToList();
		await SetBookCategoriesAsync(excludedAlumniMembersIds);
  }

  private async Task SetBookCategoriesAsync(List<int> excludedAlumniMembersIds)
  {
		var spec = new BookCategoriesSpec();
		var bookCategoriesEntity = await _bookCategoryRepository.ListAsync(spec);
		BookCategory.CalcAndSetCategoriesBooksRank(_rankingService, bookCategoriesEntity);
		BookCategory.CalcAndSetMemberCategoriesMembersRank(_rankingService, bookCategoriesEntity);
		BookCategory.AddMembersRole(bookCategoriesEntity, excludedAlumniMembersIds);
		BookCategories = _mapper.Map<List<BookCategoryDto>>(bookCategoriesEntity);
		UpdateBooksReadCount();
		OderByRankForMembersAndBooks();
	}

  private void UpdateBooksReadCount()
  {
	  foreach (var category in BookCategories)
	  {
		  foreach (var member in category.Members)
		  {
			  member.BooksReadCount = member.BooksRead!.Count(x => x.BookCategoryId == category.Id);
		  }
	  }
  }

	private void OderByRankForMembersAndBooks()
	{
		foreach (var bookCategory in BookCategories)
		{
			bookCategory.Members = bookCategory.Members.OrderBy(x => x.BooksRank).ToList();
			bookCategory.Books = bookCategory.Books!.OrderBy(x => x.Rank).ToList();
		}
	}


	private async Task<List<Member>> SetAlumniMembersAsync()
  {
	  var usersInAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);
	  var alumniUserIds = usersInAlumniRole.Select(x => x.Id).ToList();

		var alumniSpec = new MembersHavingUserIdsWithBooksSpec(alumniUserIds);
	  var alumniMembers = await _memberRepository.ListAsync(alumniSpec);
	  Member.CalcAndSetBooksRank(_rankingService, alumniMembers);
		Member.SetRoleToMembers(alumniMembers, AuthConstants.Roles.ALUMNI);		
		Alumni = _mapper.Map<List<MemberForBookDto>>(alumniMembers);

	  return alumniMembers;
  }
}
