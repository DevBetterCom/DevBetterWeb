using System.Collections.Generic;
using System.Threading;
using DevBetterWeb.Web.Interfaces;
using Xunit;
using Moq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Pages.Leaderboard;
using DevBetterWeb.Web.Services;

namespace DevBetterWeb.UnitTests.Web.Services.FilteredBookDetailsTests;

public class GetBookDetailsAsync
{
	private readonly Mock<INonCurrentMembersService> _nonCurrentMembersServiceMock;
	private readonly Mock<IBookService> _bookServiceMock;
	private readonly FilteredBookDetailsService _filteredBookDetailsService;

	public GetBookDetailsAsync()
	{
		_nonCurrentMembersServiceMock = new Mock<INonCurrentMembersService>();
		_bookServiceMock = new Mock<IBookService>();
		_filteredBookDetailsService = new FilteredBookDetailsService(_nonCurrentMembersServiceMock.Object, _bookServiceMock.Object);
	}

	[Fact]
	public async Task ReturnsNullGivenNonExistentBookId()
	{
		// Arrange
		var bookId = "1";
		_bookServiceMock.Setup(s => s.GetBookByIdAsync(int.Parse(bookId))).ReturnsAsync((BookDetailsViewModel?)null);

		// Act
		var result = await _filteredBookDetailsService.GetBookDetailsAsync(bookId);

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

		// Act
		var result = await _filteredBookDetailsService.GetBookDetailsAsync(bookId);

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
		_nonCurrentMembersServiceMock.Setup(s => s.GetUsersIdsWithoutRolesAsync()).ReturnsAsync(new List<string> { "2", "3" });
		_nonCurrentMembersServiceMock.Setup(s => s.GetNonCurrentMembersAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 2, 3 });

		// Act
		var result = await _filteredBookDetailsService.GetBookDetailsAsync(bookId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(filteredBookDetails.MembersWhoHaveRead.Count, result.MembersWhoHaveRead.Count);
	}

}
