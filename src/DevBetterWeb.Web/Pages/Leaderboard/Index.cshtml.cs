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
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
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
  private readonly IFilteredLeaderboardService _filteredLeaderboardService;

  public List<BookCategoryDto> BookCategories { get; set; } = new List<BookCategoryDto>();
	public List<int> AlumniMemberIds { get; set; } = new List<int>();
	public List<int> MemberIds { get; set; } = new List<int>();

  public IndexModel(UserManager<ApplicationUser> userManager,
      IRepository<Member> memberRepository,
      IRepository<BookCategory> bookCategoryRepository,
      IMapper mapper,
      IFilteredLeaderboardService filteredLeaderboardService)
  {
    _userManager = userManager;
    _memberRepository = memberRepository;
    _bookCategoryRepository = bookCategoryRepository;
    _mapper = mapper;
    _filteredLeaderboardService = filteredLeaderboardService;
  }

  public async Task OnGet()
  {
		var alumniMembers = await GetAlumniMembersAsync();
		AlumniMemberIds = alumniMembers.Select(x => x.Id).ToList();
		var members = await GetMembersAsync();
		MemberIds = members.Select(x => x.Id).ToList();
		await SetBookCategoriesAsync();
  }

  private async Task SetBookCategoriesAsync()
  {
		var spec = new BookCategoriesSpec();
		var bookCategoriesEntity = await _bookCategoryRepository.ListAsync(spec);
		BookCategories = _mapper.Map<List<BookCategoryDto>>(bookCategoriesEntity);

		BookCategories = await _filteredLeaderboardService.RemoveNonCurrentMembersFromLeaderBoardAsync(BookCategories);

		UpdateRanksAndReadBooksCountForMember();
		UpdateMembersReadRank();
		UpdateBooksRank();
		OrderByRankForMembersAndBooks();
  }

  private void UpdateRanksAndReadBooksCountForMember(MemberForBookDto memberWhoHaveRead, BookCategoryDto bookCategory)
  {
	  var memberToAdd = new MemberForBookDto
		  {
			  Id = memberWhoHaveRead.Id, 
			  FullName = memberWhoHaveRead.FullName,
			  BooksReadCount = memberWhoHaveRead.BooksRead!.Count(x => x.BookCategoryId == bookCategory.Id),
				UserId = memberWhoHaveRead.UserId,
		  };

	  var isAlumni = AlumniMemberIds.Contains(memberWhoHaveRead.Id);
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

	private void UpdateRanksAndReadBooksCountForMember(BookDto book, BookCategoryDto bookCategory)
  {
	  foreach (var memberWhoHaveRead in book.MembersWhoHaveRead!)
	  {
		  UpdateRanksAndReadBooksCountForMember(memberWhoHaveRead, bookCategory);
	  }
  }
	private void UpdateRanksAndReadBooksCountForMember(BookCategoryDto bookCategory)
  {
	  foreach (var book in bookCategory.Books!)
	  {
			UpdateRanksAndReadBooksCountForMember(book, bookCategory);
	  }
  }

	private void UpdateRanksAndReadBooksCountForMember()
  {
	  foreach (var bookCategory in BookCategories)
	  {
			UpdateRanksAndReadBooksCountForMember(bookCategory);
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

	private void OrderByRankForMembersAndBooks()
	{
		foreach (var bookCategory in BookCategories)
		{
			bookCategory.Members = bookCategory.Members.OrderBy(x => x.BooksRank).ToList();
			bookCategory.Alumnus = bookCategory.Alumnus.OrderBy(x => x.BooksRank).ToList();
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

	private async Task<List<Member>> GetMembersAsync()
	{
		var usersInMemberRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
		var memberUserIds = usersInMemberRole.Select(x => x.Id).ToList();

		var memberSpec = new MembersHavingUserIdsWithBooksSpec(memberUserIds);
		var members = await _memberRepository.ListAsync(memberSpec);

		return members;
	}
}
