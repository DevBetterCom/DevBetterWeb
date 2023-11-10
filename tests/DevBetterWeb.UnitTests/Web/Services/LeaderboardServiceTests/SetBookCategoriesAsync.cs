using System.Collections.Generic;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace DevBetterWeb.UnitTests.Web.Services.LeaderboardServiceTests;

public class SetBookCategoriesAsync
{
	private readonly Mock<IRankAndOrderService> _rankAndOrderServiceMock;
	private readonly Mock<IBookCategoryService> _bookCategoryServiceMock;
	private readonly Mock<IFilteredLeaderboardService> _filteredLeaderboardServiceMock;
	private readonly LeaderboardService _leaderboardService;

	public SetBookCategoriesAsync()
	{
		_rankAndOrderServiceMock = new Mock<IRankAndOrderService>();
		_bookCategoryServiceMock = new Mock<IBookCategoryService>();
		_filteredLeaderboardServiceMock = new Mock<IFilteredLeaderboardService>();
		_leaderboardService = new LeaderboardService(_rankAndOrderServiceMock.Object, _bookCategoryServiceMock.Object, _filteredLeaderboardServiceMock.Object);
	}

	[Fact]
	public async Task UpdateRanksAndOrdersGivenBookCategories()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Members = new List<MemberForBookDto>
				{
					new MemberForBookDto { Id = 1, UserId = "UserId1" },
					new MemberForBookDto { Id = 2, UserId = "UserId2" }
				}
			}
		};

		_bookCategoryServiceMock.Setup(bs => bs.GetBookCategoriesAsync()).ReturnsAsync(bookCategories);
		_filteredLeaderboardServiceMock.Setup(fs => fs.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None)).ReturnsAsync(bookCategories);

		// Act
		await _leaderboardService.SetBookCategoriesAsync();

		// Assert
		_rankAndOrderServiceMock.Verify(rs => rs.UpdateRanksAndReadBooksCountForMemberAsync(It.IsAny<List<BookCategoryDto>>()), Times.Never);
		_rankAndOrderServiceMock.Verify(rs => rs.UpdateMembersReadRank(It.IsAny<List<BookCategoryDto>>()), Times.Once);
		_rankAndOrderServiceMock.Verify(rs => rs.UpdateBooksRank(It.IsAny<List<BookCategoryDto>>()), Times.Once);
		_rankAndOrderServiceMock.Verify(rs => rs.OrderByRankForMembersAndBooks(It.IsAny<List<BookCategoryDto>>()), Times.Once);
	}

	[Fact]
	public async Task NotIncludeNonActiveMemberInRankGivenNonActiveMember()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Members = new List<MemberForBookDto>
				{
					new MemberForBookDto { Id = 1, UserId = "UserId1" },
					new MemberForBookDto { Id = 2, UserId = "UserId2" }
				}
			}
		};
		var bookCategoriesWithoutNonActiveMember = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Members = new List<MemberForBookDto>
				{
					new MemberForBookDto { Id = 2, UserId = "UserId2" }
				}
			}
		};

		_bookCategoryServiceMock.Setup(bs => bs.GetBookCategoriesAsync()).ReturnsAsync(bookCategories);
		_filteredLeaderboardServiceMock.Setup(fs => fs.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None)).ReturnsAsync(bookCategoriesWithoutNonActiveMember);

		// Act
		var result = await _leaderboardService.SetBookCategoriesAsync();

		// Assert
		Assert.Single(result[0].Members);
		Assert.Equal(2, result[0].Members[0].Id);
	}

	[Fact]
	public async Task ReturnsEmptyListGivenNoBookCategories()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>();

		_bookCategoryServiceMock.Setup(bs => bs.GetBookCategoriesAsync()).ReturnsAsync(bookCategories);
		_filteredLeaderboardServiceMock.Setup(fs => fs.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None)).ReturnsAsync(bookCategories);

		// Act
		var result = await _leaderboardService.SetBookCategoriesAsync();

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task ThrowsExceptionGivenGetBookCategoriesAsyncFails()
	{
		// Arrange
		_bookCategoryServiceMock.Setup(bs => bs.GetBookCategoriesAsync()).ThrowsAsync(new ArgumentNullException());

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => _leaderboardService.SetBookCategoriesAsync());
	}

	[Fact]
	public async Task OrdersMemberRanksCorrectlyGivenUnorderedRanks()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Members = new List<MemberForBookDto>
				{
					new MemberForBookDto { Id = 1, UserId = "UserId1", BooksRank = 2},
					new MemberForBookDto { Id = 2, UserId = "UserId2", BooksRank = 1 }
				}
			}
		};

		_bookCategoryServiceMock.Setup(bs => bs.GetBookCategoriesAsync()).ReturnsAsync(bookCategories);
		_filteredLeaderboardServiceMock.Setup(fs => fs.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None)).ReturnsAsync(bookCategories);

		_rankAndOrderServiceMock.Setup(rs => rs.OrderByRankForMembersAndBooks(It.IsAny<List<BookCategoryDto>>()))
			.Callback((List<BookCategoryDto> categories) =>
			{
				foreach (var category in categories)
				{
					category.Members = category.Members.OrderBy(m => m.BooksRank).ToList();
				}
			});

		// Act
		var result = await _leaderboardService.SetBookCategoriesAsync();

		// Assert
		Assert.Equal(2, result[0].Members.Count);
		Assert.Equal(1, result[0].Members[0].BooksRank);
		Assert.Equal(2, result[0].Members[1].BooksRank);
	}

	[Fact]
	public async Task OrdersMembersCorrectlyByBooksReadCountGivenUnorderedMembers()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>
		{
			new BookCategoryDto
			{
				Members = new List<MemberForBookDto>
				{
					new MemberForBookDto { Id = 1, UserId = "UserId1", BooksReadCount = 1, BooksRank = 2 },
					new MemberForBookDto { Id = 2, UserId = "UserId2", BooksReadCount = 3, BooksRank = 1 }
				}
			}
		};

		_bookCategoryServiceMock.Setup(bs => bs.GetBookCategoriesAsync()).ReturnsAsync(bookCategories);
		_filteredLeaderboardServiceMock.Setup(fs => fs.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None)).ReturnsAsync(bookCategories);

		_rankAndOrderServiceMock.Setup(rs => rs.OrderByRankForMembersAndBooks(It.IsAny<List<BookCategoryDto>>()))
			.Callback((List<BookCategoryDto> categories) =>
			{
				foreach (var category in categories)
				{
					category.Members = category.Members.OrderByDescending(m => m.BooksReadCount).ThenBy(m => m.BooksRank).ToList();
				}
			});

		// Act
		var result = await _leaderboardService.SetBookCategoriesAsync();

		// Assert
		Assert.Equal(2, result[0].Members.Count);
		Assert.Equal(3, result[0].Members[0].BooksReadCount);
		Assert.Equal(1, result[0].Members[0].BooksRank);
		Assert.Equal(1, result[0].Members[1].BooksReadCount);
		Assert.Equal(2, result[0].Members[1].BooksRank);
	}
}
