using System.Collections.Generic;
using DevBetterWeb.Web.Services;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.RankingServiceTests;

public class RankTests
{
	private readonly RankingService _rankingService;

	public RankTests()
	{
		_rankingService = new RankingService();
	}

	[Fact]
	public void ReturnsCorrectRanking()
	{
		// Arrange
		var items = new List<int> { 3, 1, 2 };

		// Act
		var result = _rankingService.Rank(items);

		// Assert
		Assert.Equal(1, result[3]);
		Assert.Equal(2, result[2]);
		Assert.Equal(3, result[1]);
	}

	[Fact]
	public void ReturnsCorrectRankingWhenDuplicatesPresent()
	{
		// Arrange
		var items = new List<int> { 3, 3, 1, 2, 2, 2 };

		// Act
		var result = _rankingService.Rank(items);

		// Assert
		Assert.Equal(1, result[3]);
		Assert.Equal(2, result[2]);
		Assert.Equal(3, result[1]);
	}

	[Fact]
	public void ReturnsCorrectRankingWhenAllSame()
	{
		// Arrange
		var items = new List<int> { 3, 3, 3 };

		// Act
		var result = _rankingService.Rank(items);

		// Assert
		Assert.Equal(1, result[3]);
	}

	[Fact]
	public void ReturnsEmptyRankingWhenNoItems()
	{
		// Arrange
		var items = new List<int>();

		// Act
		var result = _rankingService.Rank(items);

		// Assert
		Assert.Empty(result);
	}
}
