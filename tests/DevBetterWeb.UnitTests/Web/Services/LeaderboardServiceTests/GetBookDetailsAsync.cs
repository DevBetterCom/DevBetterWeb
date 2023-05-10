using System.Collections.Generic;
using System.Threading;
using DevBetterWeb.Web.Interfaces;
using Xunit;
using Moq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Pages.Leaderboard;
using DevBetterWeb.Web.Services;

namespace DevBetterWeb.UnitTests.Web.Services.LeaderboardServiceTests;

public class GetBookDetailsAsync
{
	private readonly Mock<IFilteredLeaderboardService> _filteredLeaderboardServiceMock;
	private readonly Mock<IBookService> _bookServiceMock;
	private readonly LeaderboardService _leaderboardService;

	public GetBookDetailsAsync()
	{
		var rankAndOrderServiceMock = new Mock<IRankAndOrderService>();
		var bookCategoryServiceMock = new Mock<IBookCategoryService>();
		_filteredLeaderboardServiceMock = new Mock<IFilteredLeaderboardService>();
		_bookServiceMock = new Mock<IBookService>();
		_leaderboardService = new LeaderboardService(rankAndOrderServiceMock.Object, bookCategoryServiceMock.Object, _bookServiceMock.Object, _filteredLeaderboardServiceMock.Object);
	}

	[Fact]
	public async Task ReturnsNullGivenNonExistentBookId()
	{
		// Arrange
		var bookId = "1";
		_bookServiceMock.Setup(s => s.GetBookByIdAsync(int.Parse(bookId))).ReturnsAsync((BookDetailsViewModel?)null);

		// Act
		var result = await _leaderboardService.GetBookDetailsAsync(bookId);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task ReturnBookDetailsGivenValidBookId()
	{
		// Arrange
		var bookId = "1";
		var bookDetails = new BookDetailsViewModel();
		_bookServiceMock.Setup(s => s.GetBookByIdAsync(int.Parse(bookId))).ReturnsAsync(bookDetails);
		_filteredLeaderboardServiceMock.Setup(s => s.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetails, CancellationToken.None)).ReturnsAsync(bookDetails);

		// Act
		var result = await _leaderboardService.GetBookDetailsAsync(bookId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(bookDetails, result);
	}

	[Fact]
	public async Task FilterNonCurrentMembersGivenValidBookId()
	{
		// Arrange
		var bookId = "1";
		var bookDetails = new BookDetailsViewModel
		{
			MembersWhoHaveRead = new List<Member> { new Member { Id = 1 }, new Member { Id = 2 }, new Member { Id = 3 } }
		};
		var filteredBookDetails = new BookDetailsViewModel
		{
			MembersWhoHaveRead = new List<Member> { new Member { Id = 1 } }
		};
		_bookServiceMock.Setup(s => s.GetBookByIdAsync(int.Parse(bookId))).ReturnsAsync(bookDetails);
		_filteredLeaderboardServiceMock.Setup(s => s.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetails, CancellationToken.None)).ReturnsAsync(filteredBookDetails);

		// Act
		var result = await _leaderboardService.GetBookDetailsAsync(bookId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(filteredBookDetails, result);
	}
}
