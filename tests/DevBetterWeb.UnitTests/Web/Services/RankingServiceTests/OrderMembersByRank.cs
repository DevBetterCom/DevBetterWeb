using System.Collections.Generic;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.RankingServiceTests;

public class OrderMembersByRank
{
	private readonly RankingService _rankingService;

	public OrderMembersByRank()
	{
		_rankingService = new RankingService();
	}

	[Fact]
	public void OrderByRankGivenMembersAreNotOrderedByRank()
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
	public void KeepOrderGivenMembersAreAlreadyOrderedByRank()
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
	public void KeepOriginalOrderGivenMembersHaveSameRank()
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
	public void ReturnEmptyListGivenMemberListIsEmpty()
	{
		// Arrange
		var members = new List<MemberForBookDto>();

		// Act
		var result = _rankingService.OrderMembersByRank(members);

		// Assert
		Assert.Empty(result);
	}
}
