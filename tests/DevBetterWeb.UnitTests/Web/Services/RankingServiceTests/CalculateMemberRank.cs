using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.RankingServiceTests;

public class CalculateMemberRank
{
	private readonly RankingService _rankingService;

	public CalculateMemberRank()
	{
		_rankingService = new RankingService();
	}

	[Fact]
	public void SetCorrectBookRankForMembersGivenMembersAndBooksReadCount()
	{
		// Arrange
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { FullName = "Alice", BooksReadCountByCategory = 3 },
			new MemberForBookDto { FullName = "Bob", BooksReadCountByCategory = 2 },
			new MemberForBookDto { FullName = "Charlie", BooksReadCountByCategory = 1 }
		};

		// Act
		_rankingService.CalculateMemberRank(members);

		// Assert
		var alice = members.First(m => m.FullName == "Alice");
		var bob = members.First(m => m.FullName == "Bob");
		var charlie = members.First(m => m.FullName == "Charlie");

		Assert.Equal(1, alice.BooksRank);
		Assert.Equal(2, bob.BooksRank);
		Assert.Equal(3, charlie.BooksRank);
	}

	[Fact]
	public void SetSameRankForAllGivenAllMembersHaveReadSameNumberOfBooks()
	{
		// Arrange
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { FullName = "Alice", BooksReadCount = 3 },
			new MemberForBookDto { FullName = "Bob", BooksReadCount = 3 },
			new MemberForBookDto { FullName = "Charlie", BooksReadCount = 3 }
		};

		// Act
		_rankingService.CalculateMemberRank(members);

		// Assert
		foreach (var member in members)
		{
			Assert.Equal(1, member.BooksRank);
		}
	}

	[Fact]
	public void SetCorrectRankForAllGivenSomeMembersHaveReadSameNumberOfBooks()
	{
		// Arrange
		var members = new List<MemberForBookDto>
		{
			new MemberForBookDto { FullName = "Alice", BooksReadCountByCategory = 3 },
			new MemberForBookDto { FullName = "Bob", BooksReadCountByCategory = 3 },
			new MemberForBookDto { FullName = "Charlie", BooksReadCountByCategory = 2 }
		};

		// Act
		_rankingService.CalculateMemberRank(members);

		// Assert
		var alice = members.First(m => m.FullName == "Alice");
		var bob = members.First(m => m.FullName == "Bob");
		var charlie = members.First(m => m.FullName == "Charlie");

		Assert.Equal(1, alice.BooksRank);
		Assert.Equal(1, bob.BooksRank);
		Assert.Equal(2, charlie.BooksRank);
	}
}
