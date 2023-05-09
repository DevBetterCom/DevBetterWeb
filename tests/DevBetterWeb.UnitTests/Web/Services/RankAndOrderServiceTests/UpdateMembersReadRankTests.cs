using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using Moq;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class UpdateMembersReadRankTests
{
	private readonly Mock<IMemberService> _mockMemberService;
	private readonly RankAndOrderService _rankAndOrderService;

	public UpdateMembersReadRankTests()
	{
		var mockRankingService = new Mock<IRankingService>();
		_mockMemberService = new Mock<IMemberService>();
		_rankAndOrderService = new RankAndOrderService(mockRankingService.Object, _mockMemberService.Object);
	}

	[Fact]
	public async Task UpdateRanksAndReadBooksCountForMemberAsync_IncludesOnlyCurrentAlumniMembers()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Id = 1,
				Books = new List<BookDto>
				{
					new BookDto
					{
						Id = 1,
						MembersWhoHaveRead = new List<MemberForBookDto>
						{
							new MemberForBookDto { Id = 1, RoleName = AuthConstants.Roles.ALUMNI, UserId = "1", FullName = "Alumni1" },
							new MemberForBookDto { Id = 2, RoleName = AuthConstants.Roles.MEMBERS, UserId = "2", FullName = "Member1" },
							new MemberForBookDto { Id = 3, RoleName = AuthConstants.Roles.ALUMNI, UserId = "3", FullName = "Alumni2" }
						}
					}
				}
			}
		};

		var activeAlumniMembers = new List<Member>
		{
			new Member { Id = 1 }
		};

		_mockMemberService.Setup(s => s.GetActiveAlumniMembersAsync()).ReturnsAsync(activeAlumniMembers);

		// Act
		await _rankAndOrderService.UpdateRanksAndReadBooksCountForMemberAsync(bookCategories);

		// Assert
		foreach (var category in bookCategories)
		{
			Assert.DoesNotContain(category.Alumnus, m => m.Id == 3);
			Assert.Contains(category.Alumnus, m => m.Id == 1);
		}
	}

	[Fact]
	public async Task UpdateRanksAndReadBooksCountForMemberAsync_IncludesOnlyCurrentMembers()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Id = 1,
				Books = new List<BookDto>
				{
					new BookDto
					{
						Id = 1,
						MembersWhoHaveRead = new List<MemberForBookDto>
						{
							new MemberForBookDto { Id = 1, RoleName = AuthConstants.Roles.MEMBERS, UserId = "1", FullName = "Member1", BooksRead = new List<BookDto>() },
							new MemberForBookDto { Id = 2, RoleName = AuthConstants.Roles.MEMBERS, UserId = "2", FullName = "Member2", BooksRead = new List<BookDto>() },
							new MemberForBookDto { Id = 3, RoleName = AuthConstants.Roles.MEMBERS, UserId = "3", FullName = "Member3", BooksRead = new List<BookDto>() }
						}
					}
				}
			}
		};

		var activeMembers = new List<Member>
		{
			new Member { Id = 1 },
			new Member { Id = 3 }
		};

		_mockMemberService.Setup(s => s.GetActiveMembersAsync()).ReturnsAsync(activeMembers);

		// Act
		await _rankAndOrderService.UpdateRanksAndReadBooksCountForMemberAsync(bookCategories);

		// Assert
		foreach (var category in bookCategories)
		{
			Assert.DoesNotContain(category.Members, m => m.Id == 2);
			Assert.Contains(category.Members, m => m.Id == 1);
			Assert.Contains(category.Members, m => m.Id == 3);
		}
	}

}
