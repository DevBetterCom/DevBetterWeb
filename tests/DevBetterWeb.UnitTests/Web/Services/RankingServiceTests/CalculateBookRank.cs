using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.RankingServiceTests;

public class CalculateBookRank
{
	private readonly RankingService _rankingService;

	public CalculateBookRank()
	{
		_rankingService = new RankingService();
	}

	[Fact]
	public void UpdateBooksRankGivenBooksAndMembersWhoHaveReadCount()
	{
		// Arrange
		var books = new List<BookDto>
		{
			new BookDto { MembersWhoHaveReadCount = 3 },
			new BookDto { MembersWhoHaveReadCount = 1 },
			new BookDto { MembersWhoHaveReadCount = 2 }
		};

		// Act
		_rankingService.CalculateBookRank(books);

		// Assert
		Assert.Equal(1, books.Single(b => b.MembersWhoHaveReadCount == 3).Rank);
		Assert.Equal(2, books.Single(b => b.MembersWhoHaveReadCount == 2).Rank);
		Assert.Equal(3, books.Single(b => b.MembersWhoHaveReadCount == 1).Rank);
	}

	[Fact]
	public void SetSameRankForAllGivenAllBooksReadSameNumberOfTimes()
	{
		// Arrange
		var books = new List<BookDto>
		{
			new BookDto { MembersWhoHaveReadCount = 3 },
			new BookDto { MembersWhoHaveReadCount = 3 },
			new BookDto { MembersWhoHaveReadCount = 3 }
		};

		// Act
		_rankingService.CalculateBookRank(books);

		// Assert
		foreach (var book in books)
		{
			Assert.Equal(1, book.Rank);
		}
	}

	[Fact]
	public void SetCorrectRankForAllGivenSomeBooksReadSameNumberOfTimes()
	{
		// Arrange
		var books = new List<BookDto>
		{
			new BookDto { MembersWhoHaveReadCount = 3 },
			new BookDto { MembersWhoHaveReadCount = 3 },
			new BookDto { MembersWhoHaveReadCount = 2 }
		};

		// Act
		_rankingService.CalculateBookRank(books);

		// Assert
		Assert.Equal(1, books.First(b => b.MembersWhoHaveReadCount == 3).Rank);
		Assert.Equal(1, books.First(b => b.MembersWhoHaveReadCount == 3).Rank);
		Assert.Equal(2, books.First(b => b.MembersWhoHaveReadCount == 2).Rank);
	}
}
