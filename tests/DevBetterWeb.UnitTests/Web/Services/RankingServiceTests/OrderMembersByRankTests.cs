using System.Collections.Generic;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.RankingServiceTests;

public class OrderMembersByRankTests
{
	private readonly RankingService _rankingService;

	public OrderMembersByRankTests()
	{
		_rankingService = new RankingService();
	}

	[Fact]
	public void WhenMembersAreNotOrderedByRank_OrdersByRank()
	{
		// Arrange
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { FullName = "Alice", BooksRank = 3 },
			new MemberForBookDto { FullName = "Bob", BooksRank = 1 },
			new MemberForBookDto { FullName = "Charlie", BooksRank = 2 }
		};

		// Act
		var result = _rankingService.OrderMembersByRank(members);

		// Assert
		Assert.Equal("Bob", result[0].FullName);
		Assert.Equal("Charlie", result[1].FullName);
		Assert.Equal("Alice", result[2].FullName);
	}

	[Fact]
	public void WhenMembersAreAlreadyOrderedByRank_KeepsOrder()
	{
		// Arrange
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { FullName = "Alice", BooksRank = 1 },
			new MemberForBookDto { FullName = "Bob", BooksRank = 2 },
			new MemberForBookDto { FullName = "Charlie", BooksRank = 3 }
		};

		// Act
		var result = _rankingService.OrderMembersByRank(members);

		// Assert
		Assert.Equal("Alice", result[0].FullName);
		Assert.Equal("Bob", result[1].FullName);
		Assert.Equal("Charlie", result[2].FullName);
	}

	[Fact]
	public void WhenMembersHaveSameRank_KeepsOriginalOrder()
	{
		// Arrange
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { FullName = "Alice", BooksRank = 1, BooksReadCount = 3 },
			new MemberForBookDto { FullName = "Bob", BooksRank = 1, BooksReadCount = 2 },
			new MemberForBookDto { FullName = "Charlie", BooksRank = 2 }
		};

		// Act
		var result = _rankingService.OrderMembersByRank(members);

		// Assert
		Assert.Equal("Alice", result[0].FullName);
		Assert.Equal("Bob", result[1].FullName);
	}

	[Fact]
	public void WhenMemberListIsEmpty_ReturnsEmptyList()
	{
		// Arrange
		var members = new List<MemberForBookDto>();

		// Act
		var result = _rankingService.OrderMembersByRank(members);

		// Assert
		Assert.Empty(result);
	}
}
