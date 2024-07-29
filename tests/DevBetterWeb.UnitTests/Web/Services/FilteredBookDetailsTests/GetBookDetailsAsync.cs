using System.Collections.Generic;
using System.Threading;
using DevBetterWeb.Web.Interfaces;
using Xunit;
using NSubstitute;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Pages.Leaderboard;
using DevBetterWeb.Web.Services;

namespace DevBetterWeb.UnitTests.Web.Services.FilteredBookDetailsTests;

public class GetBookDetailsAsync
{
	private readonly INonCurrentMembersService _nonCurrentMembersServiceMock;
	private readonly IBookService _bookServiceMock;
	private readonly FilteredBookDetailsService _filteredBookDetailsService;

	public GetBookDetailsAsync()
	{
		_nonCurrentMembersServiceMock = Substitute.For<INonCurrentMembersService>();
		_bookServiceMock = Substitute.For<IBookService>();
		_filteredBookDetailsService = new FilteredBookDetailsService(_nonCurrentMembersServiceMock, _bookServiceMock);
	}

	[Fact]
	public async Task ReturnsNullGivenNonExistentBookId()
	{
		// Arrange
		var bookId = "1";
		_bookServiceMock.GetBookByIdAsync(int.Parse(bookId)).Returns((BookDetailsViewModel?)null);

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
		_bookServiceMock.GetBookByIdAsync(int.Parse(bookId)).Returns(bookDetails);

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
		_bookServiceMock.GetBookByIdAsync(int.Parse(bookId)).Returns(bookDetails);
		_nonCurrentMembersServiceMock.GetUsersIdsWithoutRolesAsync().Returns(new List<string> { "2", "3" });
		_nonCurrentMembersServiceMock.GetNonCurrentMembersAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>()).Returns(new List<int> { 2, 3 });

		// Act
		var result = await _filteredBookDetailsService.GetBookDetailsAsync(bookId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(filteredBookDetails.MembersWhoHaveRead.Count, result.MembersWhoHaveRead.Count);
	}

}
