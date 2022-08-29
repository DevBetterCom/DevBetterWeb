using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Core.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.RankingServiceTests;

public class RankIntegers
{
	[Fact]
  public void EmptyReturnsEmptyDictionary()
  {
    var actual = RankingService<int>.Rank(new List<int>());
    Assert.False(actual.Any());
  }

  [Theory]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void SingleIntegerHasRankOfOne(int value)
  {
    var rankings = RankingService<int>.Rank(new List<int> { value });
    Assert.Equal(1, rankings[value]);
  }

  [Fact]
  public void ContiguousIntegersRankedProperly()
  {
    var numbersToRank = new List<int> { 1, 2, 3 };
    var rankings = RankingService<int>.Rank(numbersToRank);
    Assert.Equal(1, rankings[3]);
    Assert.Equal(2, rankings[2]);
    Assert.Equal(3, rankings[1]);
  }

  [Fact]
  public void NonContiguousIntegersRankedProperly()
  {
    var numbersToRank = new List<int> { 1, 3, 5 };
    var rankings = RankingService<int>.Rank(numbersToRank);
    Assert.Equal(1, rankings[5]);
    Assert.Equal(2, rankings[3]);
    Assert.Equal(3, rankings[1]);
  }

  [Fact]
  public void DuplicateIntegersHaveSameRank()
  {
    var numbersToRank = new List<int> { 1, 1, 2, 2, 3 };
    // Expected Ranking Result from this series:
    // Rank  Entry
    // 1      3
    // 2      2
    // 2      2
    // 4      1
    // 4      1
    // 4      1
    var rankings = RankingService<int>.Rank(numbersToRank);
    Assert.Equal(1, rankings[3]);
    Assert.Equal(2, rankings[2]);
    Assert.Equal(3, rankings[1]);
  }
}
