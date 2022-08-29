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
		BookCategories = _mapper.Map<List<BookCategoryDto>>(bookCategoriesEntity);

		UpdateRanksAndReadBooksCountForMember(alumniMembersIds);
		UpdateMembersReadRank();
		UpdateBooksRank();
		OderByRankForMembersAndBooks();
	}
  private void UpdateRanksAndReadBooksCountForMember(List<int> alumniMembersIds, MemberForBookDto memberWhoHaveRead, BookCategoryDto bookCategory)
  {
	  var memberToAdd = new MemberForBookDto
		  {
			  Id = memberWhoHaveRead.Id, 
			  FullName = memberWhoHaveRead.FullName,
			  BooksReadCount = memberWhoHaveRead.BooksRead!.Count(x => x.BookCategoryId == bookCategory.Id),
				UserId = memberWhoHaveRead.UserId,
		  };

	  var isAlumni = alumniMembersIds.Contains(memberWhoHaveRead.Id);
		if (isAlumni)
		{
			if (bookCategory.Alumnus.Any(m => m.Id == memberWhoHaveRead.Id))
			{
				return;
			}

			memberToAdd.RoleName = AuthConstants.Roles.ALUMNI;
			bookCategory.Alumnus.Add(memberToAdd);
		}
	  else
		{
			if (bookCategory.Members.Any(m => m.Id == memberWhoHaveRead.Id))
			{
				return;
			}

			memberToAdd.RoleName = AuthConstants.Roles.MEMBERS;
			bookCategory.Members.Add(memberToAdd);
		}
  }

	private void UpdateRanksAndReadBooksCountForMember(List<int> alumniMembersIds, BookDto book, BookCategoryDto bookCategory)
  {
	  foreach (var memberWhoHaveRead in book.MembersWhoHaveRead!)
	  {
		  UpdateRanksAndReadBooksCountForMember(alumniMembersIds, memberWhoHaveRead, bookCategory);
	  }
  }
	private void UpdateRanksAndReadBooksCountForMember(List<int> alumniMembersIds, BookCategoryDto bookCategory)
  {
	  foreach (var book in bookCategory.Books!)
	  {
			UpdateRanksAndReadBooksCountForMember(alumniMembersIds, book, bookCategory);
	  }
  }

	private void UpdateRanksAndReadBooksCountForMember(List<int> alumniMembersIds)
  {
	  foreach (var bookCategory in BookCategories)
	  {
			UpdateRanksAndReadBooksCountForMember(alumniMembersIds, bookCategory);
	  }
  }

	private void UpdateMembersReadRank()
	{
		foreach (var category in BookCategories)
		{
			CalcMemberRank(category.Members);
			CalcMemberRank(category.Alumnus);
		}
	}

	private void CalcMemberRank(List<MemberForBookDto> members)
	{
		var memberRanks = RankingService<int>.Rank(members.Select(m => m.BooksReadCount));
		foreach (var member in members)
		{
			member.BooksRank =
				memberRanks[member.BooksReadCount];
		}
	}

	private void UpdateBooksRank()
  {
	  foreach (var category in BookCategories)
	  {
		  CalcBookRank(category.Books!);
	  }
  }

  private void CalcBookRank(List<BookDto> books)
  {
	  var bookRanks = RankingService<int>.Rank(books.Select(m => m.MembersWhoHaveReadCount));
	  books.ForEach(m => m.Rank = bookRanks[m.MembersWhoHaveReadCount]);
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
