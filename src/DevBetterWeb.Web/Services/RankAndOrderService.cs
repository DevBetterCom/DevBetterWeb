using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Services
{
	public class RankAndOrderService : IRankAndOrderService
	{
		private readonly IRankingService _rankingService;
		private readonly IMemberService _memberService;

		public RankAndOrderService(IRankingService rankingService, IMemberService memberService)
		{
			_rankingService = rankingService;
			_memberService = memberService;
		}

		public async Task UpdateRanksAndReadBooksCountForMemberAsync(List<BookCategoryDto> bookCategories)
		{
			foreach (var bookCategory in bookCategories)
			{
				await UpdateRanksAndReadBooksCountForMemberAsync(bookCategory);
			}
		}

		public void UpdateMembersReadRank(List<BookCategoryDto> bookCategories)
		{
			foreach (var category in bookCategories)
			{
				if (category.Books.Count <= 0)
				{
					continue;
				}
				_rankingService.CalculateMemberRank(category.Members);
				_rankingService.CalculateMemberRank(category.Alumnus);
			}
		}

		public void UpdateBooksRank(List<BookCategoryDto> bookCategories)
		{
			foreach (var category in bookCategories)
			{
				_rankingService.CalculateBookRank(category.Books!);
			}
		}

		public void OrderByRankForMembersAndBooks(List<BookCategoryDto> bookCategories)
		{
			foreach (var bookCategory in bookCategories)
			{
				bookCategory.Members = _rankingService.OrderMembersByRank(bookCategory.Members);
				bookCategory.Alumnus = _rankingService.OrderMembersByRank(bookCategory.Alumnus);
				bookCategory.Books = _rankingService.OrderBooksByRank(bookCategory.Books!);
			}
		}

		private async Task UpdateRanksAndReadBooksCountForMemberAsync(BookDto book, BookCategoryDto bookCategory)
		{
			foreach (var memberWhoHaveRead in book.MembersWhoHaveRead!)
			{
				await UpdateRanksAndReadBooksCountForMemberAsync(memberWhoHaveRead, bookCategory);
				book.MembersWhoHaveReadCount = book.MembersWhoHaveRead.Count;
			}
		}

		private async Task UpdateRanksAndReadBooksCountForMemberAsync(BookCategoryDto bookCategory)
		{
			foreach (var book in bookCategory.Books!)
			{
				await UpdateRanksAndReadBooksCountForMemberAsync(book, bookCategory);
			}
		}

		private async Task UpdateRanksAndReadBooksCountForMemberAsync(MemberForBookDto memberWhoHaveRead, BookCategoryDto bookCategory)
		{
			var memberToAdd = new MemberForBookDto
			{
				Id = memberWhoHaveRead.Id,
				FullName = memberWhoHaveRead.FullName,
				BooksReadCount = memberWhoHaveRead.BooksRead!.Count(x => x.BookCategoryId == bookCategory.Id),
				UserId = memberWhoHaveRead.UserId,
			};

			List<Member> alumniMembers = await _memberService.GetActiveAlumniMembersAsync();
			List<int> alumniMemberIds = alumniMembers.Select(x => x.Id).ToList();

			var isAlumni = alumniMemberIds.Contains(memberWhoHaveRead.Id);
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
				var member = bookCategory.Members.FirstOrDefault(m => m.Id == memberWhoHaveRead.Id);
				if (member != null)
				{
					member.RoleName = AuthConstants.Roles.MEMBERS;
					return;
				}

				memberToAdd.RoleName = AuthConstants.Roles.MEMBERS;
				bookCategory.Members.Add(memberToAdd);
			}
		}
	}
}
