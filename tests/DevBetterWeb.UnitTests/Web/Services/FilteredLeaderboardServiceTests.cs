using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Moq;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services;

public class FilteredLeaderboardServiceTests
{
	private readonly Mock<INonCurrentMembersService> _nonCurrentMembersServiceMock;

	public FilteredLeaderboardServiceTests()
	{
		_nonCurrentMembersServiceMock = new Mock<INonCurrentMembersService>();
	}

	[Fact]
	public async Task RemoveNonCurrentMembersFromLeaderBoardAsync_RemovesNonCurrentMembers()
	{
		// Arrange
		var bookCategories = CreateSampleBookCategories();
		List<string> nonUsersId = new List<string> { "3" };
		List<int> nonMembersId = new List<int> { 3 };

		_nonCurrentMembersServiceMock.Setup(service => service.GetUsersIdsWithoutRolesAsync())
				.ReturnsAsync(nonUsersId);
		_nonCurrentMembersServiceMock.Setup(service => service.GetNonCurrentMembersAsync(nonUsersId, CancellationToken.None))
				.ReturnsAsync(nonMembersId);

		var service = new FilteredLeaderboardService(_nonCurrentMembersServiceMock.Object);

		// Act
		var filteredBookCategories = await service.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories);

		// Assert
		Assert.All(filteredBookCategories, bookCategory =>
		{
			Assert.DoesNotContain(bookCategory.Members, member => nonMembersId.Contains(member.Id));
			Assert.All(bookCategory.Books!, book =>
			{
				Assert.DoesNotContain(book.MembersWhoHaveRead, member => nonMembersId.Contains(member.Id));
			});
		});
	}

	private List<BookCategoryDto> CreateSampleBookCategories()
	{
		return new List<BookCategoryDto>
					{
							new()
							{
									Id = 1,
									Title = "Category 1",
									Members = new List<MemberForBookDto>
									{
											new() { Id = 1, FullName = "Member 1" },
											new() { Id = 2, FullName = "Member 2" },
											new() { Id = 3, FullName = "Non-Member" }
									},
									Books = new List<BookDto>
									{
											new BookDto
											{
													Id = 1,
													Title = "Book 1",
													Author = "Author 1",
													MembersWhoHaveRead = new List<MemberForBookDto>
													{
															new() { Id = 1, FullName = "Member 1" },
															new() { Id = 2, FullName = "Member 2" },
															new() { Id = 3, FullName = "Non-Member" }
													}
											}
									}
							}
					};
	}
}
