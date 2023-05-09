using System.Collections.Generic;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.RankingServiceTests;

public class OrderBooksByRank
{
	private readonly RankingService _rankingService;

	public OrderBooksByRank()
	{
		_rankingService = new RankingService();
	}

	[Fact]
	public void OrderByRankGivenBooksAreNotOrderedByRank()
	{
		// Arrange
		var books = new List<BookDto>
		{
			new BookDto { Rank = 3 },
			new BookDto { Rank = 1 },
			new BookDto { Rank = 2 }
		};

		// Act
		var result = _rankingService.OrderBooksByRank(books);

		// Assert
		Assert.Equal(1, result[0].Rank);
		Assert.Equal(2, result[1].Rank);
		Assert.Equal(3, result[2].Rank);
	}

	[Fact]
	public void KeepOrderGivenBooksAreAlreadyOrderedByRank()
	{
		// Arrange
		var books = new List<BookDto>
		{
			new BookDto { Rank = 1 },
			new BookDto { Rank = 2 },
			new BookDto { Rank = 3 }
		};

		// Act
		var result = _rankingService.OrderBooksByRank(books);

		// Assert
		Assert.Equal(1, result[0].Rank);
		Assert.Equal(2, result[1].Rank);
		Assert.Equal(3, result[2].Rank);
	}

	[Fact]
	public void KeepOriginalOrderGivenBooksHaveSameRank()
	{
		// Arrange
		var books = new List<BookDto>
		{
			new BookDto { Rank = 1, MembersWhoHaveReadCount = 3 },
			new BookDto { Rank = 1, MembersWhoHaveReadCount = 2 },
			new BookDto { Rank = 2 }
		};

		// Act
		var result = _rankingService.OrderBooksByRank(books);

		// Assert
		Assert.Equal(3, result[0].MembersWhoHaveReadCount);
		Assert.Equal(2, result[1].MembersWhoHaveReadCount);
	}

	[Fact]
	public void ReturnEmptyListGivenBookListIsEmpty()
	{
		// Arrange
		var books = new List<BookDto>();

		// Act
		var result = _rankingService.OrderBooksByRank(books);

		// Assert
		Assert.Empty(result);
	}
}
