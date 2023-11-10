﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Moq;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.FilteredLeaderboardTests;

public class FilteredLeaderboardServiceTests
{
	private readonly Mock<INonCurrentMembersService> _nonCurrentMembersServiceMock;
	private readonly Mock<IMemberService> _memberServiceMock;

	public FilteredLeaderboardServiceTests()
	{
		_nonCurrentMembersServiceMock = new Mock<INonCurrentMembersService>();
		_memberServiceMock = new Mock<IMemberService>();
	}

	[Fact]
	public async Task RemovesNonCurrentMembersGivenBookCategories()
	{
		// Arrange
		var bookCategories = CreateSampleBookCategories();
		List<string> nonUsersId = new List<string> { "3" };
		List<int> nonMembersId = new List<int> { 3 };

		_nonCurrentMembersServiceMock.Setup(service => service.GetUsersIdsWithoutRolesAsync())
				.ReturnsAsync(nonUsersId);
		_nonCurrentMembersServiceMock.Setup(service => service.GetNonCurrentMembersAsync(nonUsersId, CancellationToken.None))
				.ReturnsAsync(nonMembersId);

		_memberServiceMock.Setup(service => service.GetActiveAlumniMembersAsync())
				.ReturnsAsync(new List<Member>());

		var service = new FilteredLeaderboardService(_nonCurrentMembersServiceMock.Object, _memberServiceMock.Object);

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
											new() { Id = 1, FullName = "Member 1", BooksReadCountByCategory = 1 },
											new() { Id = 2, FullName = "Member 2", BooksReadCountByCategory = 1 },
											new() { Id = 3, FullName = "Non-Member", BooksReadCountByCategory = 1 }
									},
									Books = new List<BookDto>
									{
											new BookDto
											{
													Id = 1,
													Title = "Book 1",
													Author = "Author 1",
													MembersWhoHaveReadCount = 3,
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
