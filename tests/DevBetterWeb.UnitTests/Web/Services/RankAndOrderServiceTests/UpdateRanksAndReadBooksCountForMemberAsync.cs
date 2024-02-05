using System.Collections.Generic;
using DevBetterWeb.Web.Interfaces;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using NSubstitute;
using System.Linq;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class UpdateRanksAndReadBooksCountForMemberAsync
{
	private readonly IMemberService _memberService;
	private readonly RankAndOrderService _rankAndOrderService;

	public UpdateRanksAndReadBooksCountForMemberAsync()
	{
		var mockRankingService = Substitute.For<IRankingService>();
		_memberService = Substitute.For<IMemberService>();
		_rankAndOrderService = new RankAndOrderService(mockRankingService, _memberService);
	}

	[Fact]
	public async Task UpdateMembersWhoHaveReadCountGivenBookCategories()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Books = new List<BookDto>
				{
					new BookDto
					{
						MembersWhoHaveRead = new List<MemberForBookDto> { new MemberForBookDto(), new MemberForBookDto() }
					},
					new BookDto
					{
						MembersWhoHaveRead = new List<MemberForBookDto>
						{
							new MemberForBookDto(), new MemberForBookDto(), new MemberForBookDto()
						}
					}
				}
			}
		};
		_memberService.GetActiveAlumniMembersAsync().Returns(new List<Member> { });

		// Act
		await _rankAndOrderService.UpdateRanksAndReadBooksCountForMemberAsync(bookCategories);

		// Assert
		Assert.Equal(2, bookCategories[0].Books[0].MembersWhoHaveReadCount);
		Assert.Equal(3, bookCategories[0].Books[1].MembersWhoHaveReadCount);
	}

	[Fact]
	public async Task UpdateAlumniMembersGivenBookCategoriesAndActiveAlumniMembers()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Books = new List<BookDto>
				{
					new BookDto
					{
						MembersWhoHaveRead = new List<MemberForBookDto>
						{
							new MemberForBookDto { Id = 1 }, new MemberForBookDto { Id = 2 }
						}
					},
					new BookDto
					{
						MembersWhoHaveRead = new List<MemberForBookDto>
						{
							new MemberForBookDto { Id = 3 },
							new MemberForBookDto { Id = 4 },
							new MemberForBookDto { Id = 1 }
						}
					}
				}
			}
		};
		_memberService.GetActiveAlumniMembersAsync()
			.Returns(new List<Member> { new Member { Id = 1 }, new Member { Id = 2 } });

		// Act
		await _rankAndOrderService.UpdateRanksAndReadBooksCountForMemberAsync(bookCategories);

		// Assert
		Assert.Equal(2, bookCategories[0].Alumnus.Count);
		Assert.Contains(bookCategories[0].Alumnus, m => m.Id == 1);
		Assert.Contains(bookCategories[0].Alumnus, m => m.Id == 2);
		Assert.Equal(AuthConstants.Roles.ALUMNI, bookCategories[0].Alumnus[0].RoleName);
		Assert.Equal(AuthConstants.Roles.ALUMNI, bookCategories[0].Alumnus[1].RoleName);
	}

	[Fact]
	public async Task UpdateMemberRoleGivenBookCategoriesAndActiveAlumniMembers()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Books = new List<BookDto>
				{
					new BookDto
					{
						MembersWhoHaveRead = new List<MemberForBookDto>
						{
							new MemberForBookDto { Id = 1 }, new MemberForBookDto { Id = 2 }
						}
					},
					new BookDto
					{
						MembersWhoHaveRead = new List<MemberForBookDto>
						{
							new MemberForBookDto { Id = 3 },
							new MemberForBookDto { Id = 4 },
							new MemberForBookDto { Id = 1 }
						}
					}
				}
			}
		};
		_memberService.GetActiveAlumniMembersAsync()
			.Returns(new List<Member> { new Member { Id = 5 }, new Member { Id = 6 } });

		// Act
		await _rankAndOrderService.UpdateRanksAndReadBooksCountForMemberAsync(bookCategories);

		// Assert
		Assert.Equal(4, bookCategories[0].Members.Count);
		Assert.True(bookCategories[0].Members.All(m => m.RoleName == AuthConstants.Roles.MEMBERS));
	}
}
