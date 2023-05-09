using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.RankingServiceTests;

public class CalculateBookRankTests
{
	private readonly RankingService _rankingService;

	public CalculateBookRankTests()
	{
		_rankingService = new RankingService();
	}

	[Fact]
	public void UpdatesBooksRank()
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
	public void WhenAllBooksReadSameNumberOfTimes_SetsSameRankForAll()
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
	public void WhenSomeBooksReadSameNumberOfTimes_SetsCorrectRankForAll()
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
