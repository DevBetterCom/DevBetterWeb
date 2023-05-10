using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Pages.Leaderboard;
using DevBetterWeb.Web.Services;
using Moq;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.FilteredLeaderboardTests;

public class RemoveNonCurrentMembersFromBookDetailsAsync
{
	private readonly Mock<INonCurrentMembersService> _mockNonCurrentMembersService;
	private readonly FilteredLeaderboardService _yourService;

	public RemoveNonCurrentMembersFromBookDetailsAsync()
	{
		_mockNonCurrentMembersService = new Mock<INonCurrentMembersService>();
		_yourService = new FilteredLeaderboardService(_mockNonCurrentMembersService.Object);
	}

	[Fact]
	public async Task RemoveNonCurrentMembersGivenNonCurrentMembersInBookDetailsViewModel()
	{
		// Arrange
		var bookDetailsViewModel = new BookDetailsViewModel
		{
			MembersWhoHaveRead = new List<Member> { new Member { Id = 1 }, new Member { Id = 2 }, new Member { Id = 3 } }
		};
		_mockNonCurrentMembersService.Setup(s => s.GetUsersIdsWithoutRolesAsync())
			.ReturnsAsync(new List<string> { "1", "2" });
		_mockNonCurrentMembersService
			.Setup(s => s.GetNonCurrentMembersAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<int> { 1, 2 });

		// Act
		var result = await _yourService.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetailsViewModel);

		// Assert
		Assert.Single(result.MembersWhoHaveRead);
		Assert.Equal(3, result.MembersWhoHaveRead.First().Id);
	}

	[Fact]
	public async Task NotRemoveCurrentMembersGivenCurrentMembersInBookDetailsViewModel()
	{
		// Arrange
		var bookDetailsViewModel = new BookDetailsViewModel
		{
			MembersWhoHaveRead = new List<Member> { new Member { Id = 1 }, new Member { Id = 2 }, new Member { Id = 3 } }
		};
		_mockNonCurrentMembersService.Setup(s => s.GetUsersIdsWithoutRolesAsync())
			.ReturnsAsync(new List<string> { "4", "5" });
		_mockNonCurrentMembersService
			.Setup(s => s.GetNonCurrentMembersAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<int> { 4, 5 });

		// Act
		var result = await _yourService.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetailsViewModel);

		// Assert
		Assert.Equal(3, result.MembersWhoHaveRead.Count);
	}

	[Fact]
	public async Task ReturnSameBookDetailsGivenNoNonCurrentMembersInBookDetailsViewModel()
	{
		// Arrange
		var bookDetailsViewModel = new BookDetailsViewModel
		{
			MembersWhoHaveRead = new List<Member> { new Member { Id = 1 }, new Member { Id = 2 }, new Member { Id = 3 } }
		};
		_mockNonCurrentMembersService.Setup(s => s.GetUsersIdsWithoutRolesAsync()).ReturnsAsync(new List<string> { });
		_mockNonCurrentMembersService
			.Setup(s => s.GetNonCurrentMembersAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<int> { });

		// Act
		var result = await _yourService.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetailsViewModel);

		// Assert
		Assert.Equal(3, result.MembersWhoHaveRead.Count);
	}

	[Fact]
	public async Task HandleEmptyMembersListGivenBookDetailsViewModelWithNoMembers()
	{
		// Arrange
		var bookDetailsViewModel = new BookDetailsViewModel { MembersWhoHaveRead = new List<Member> { } };
		_mockNonCurrentMembersService.Setup(s => s.GetUsersIdsWithoutRolesAsync()).ReturnsAsync(new List<string> { "1", "2" });
		_mockNonCurrentMembersService.Setup(s => s.GetNonCurrentMembersAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 1, 2 });

		// Act
		var result = await _yourService.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetailsViewModel);

		// Assert
		Assert.Empty(result.MembersWhoHaveRead);
	}

	[Fact]
	public async Task RemoveAllMembersGivenAllNonCurrentMembersInBookDetailsViewModel()
	{
		// Arrange
		var bookDetailsViewModel = new BookDetailsViewModel
		{
			MembersWhoHaveRead = new List<Member> { new Member { Id = 1 }, new Member { Id = 2 }, new Member { Id = 3 } }
		};
		_mockNonCurrentMembersService.Setup(s => s.GetUsersIdsWithoutRolesAsync()).ReturnsAsync(new List<string> { "1", "2", "3" });
		_mockNonCurrentMembersService.Setup(s => s.GetNonCurrentMembersAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 1, 2, 3 });

		// Act
		var result = await _yourService.RemoveNonCurrentMembersFromBookDetailsAsync(bookDetailsViewModel);

		// Assert
		Assert.Empty(result.MembersWhoHaveRead);
	}
}

