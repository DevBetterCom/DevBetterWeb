using System.Collections.Generic;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using Moq;
using System.Linq;
using System;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class UpdateMembersReadRank
{
	private readonly Mock<IRankingService> _mockRankingService;
	private readonly RankAndOrderService _rankAndOrderService;

	public UpdateMembersReadRank()
	{
		_mockRankingService = new Mock<IRankingService>();
		var mockMemberService = new Mock<IMemberService>();
		_rankAndOrderService = new RankAndOrderService(_mockRankingService.Object, mockMemberService.Object);
	}

	private List<BookCategoryDto> GetTestBookCategories()
	{
		return new List<BookCategoryDto>
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
	}

	[Fact]
	public void CalculateMemberRankGivenBookCategoriesWithMembersAndAlumni()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();

		// Act
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);

		// Assert
		foreach (var category in bookCategories)
		{
			_mockRankingService.Verify(rs => rs.CalculateMemberRank(category.Members), Times.AtLeastOnce());
			_mockRankingService.Verify(rs => rs.CalculateMemberRank(category.Alumnus), Times.AtLeastOnce());
		}
	}

	[Fact]
	public void NotCallCalculateMemberRankGivenBookCategoriesWithoutBooks()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto> { new BookCategoryDto() };

		// Act
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateMemberRank(It.IsAny<List<MemberForBookDto>>()), Times.Never());
	}


	[Fact]
	public void CalculateMemberRankForEachCategoryGivenMultipleBookCategories()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();
		bookCategories.Add(GetTestBookCategories().First());

		// Act
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateMemberRank(It.IsAny<List<MemberForBookDto>>()), Times.Exactly(bookCategories.Count * 2));
	}

	[Fact]
	public void AssignCorrectRanksGivenBookCountsStartingFromHigherNumber()
	{
		// Arrange
		var rankingService = new RankingService();
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { Id = 1, UserId = "User1", BooksReadCountByCategory = 5 },
			new MemberForBookDto { Id = 2, UserId = "User2", BooksReadCountByCategory = 6 },
			new MemberForBookDto { Id = 3, UserId = "User3", BooksReadCountByCategory = 7 },
			new MemberForBookDto { Id = 4, UserId = "User4", BooksReadCountByCategory = 8 },
		};

		// Act
		rankingService.CalculateMemberRank(members);

		// Assert
		Assert.Equal(4, members[0].BooksRank);
		Assert.Equal(3, members[1].BooksRank);
		Assert.Equal(2, members[2].BooksRank);
		Assert.Equal(1, members[3].BooksRank);
	}

	[Fact]
	public void AssignCorrectRanksGivenDuplicatedBookCounts()
	{
		// Arrange
		var rankingService = new RankingService();
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { Id = 1, UserId = "User1", BooksReadCountByCategory = 5 },
			new MemberForBookDto { Id = 2, UserId = "User2", BooksReadCountByCategory = 6 },
			new MemberForBookDto { Id = 3, UserId = "User3", BooksReadCountByCategory = 8 },
			new MemberForBookDto { Id = 4, UserId = "User4", BooksReadCountByCategory = 8 },
		};

		// Act
		rankingService.CalculateMemberRank(members);

		// Assert
		Assert.Equal(3, members[0].BooksRank);
		Assert.Equal(2, members[1].BooksRank);
		Assert.Equal(1, members[2].BooksRank);
		Assert.Equal(1, members[3].BooksRank);
	}

	[Fact]
	public void ThrowExceptionGivenNullBookCategories()
	{
		// Arrange
		List<BookCategoryDto> bookCategories = null!;

		// Act & Assert
		Assert.Throws<NullReferenceException>(() => _rankAndOrderService.UpdateMembersReadRank(bookCategories));
	}

	[Fact]
	public void NotCallCalculateMemberRankGivenEmptyBookCategories()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>();

		// Act
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateMemberRank(It.IsAny<List<MemberForBookDto>>()), Times.Never());
	}
}
