using System;
using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Specs;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Specs;

public class ArchiveVideoByPageSpecEvaluate
{
  private readonly IEnumerable<ArchiveVideo> _archiveVideos;

  public ArchiveVideoByPageSpecEvaluate()
  {
    _archiveVideos = new ArchiveVideo[]
    {
      new()
      {
        Id = 1,
        Title = "Video One",
        Description = "# Video about using markdown in ASP.NET Core",
        DateCreated = new DateTime(2019, 3, 8),
      },
      new()
      {
        Id = 2,
        Title = "Video Two",
        DateCreated = new DateTime(2019, 3, 15)
      }
    };
  }

  [Fact]
  public void ReturnsUnfilteredListGivenNullSearchExpression()
  {
    var sut = new ArchiveVideoByPageSpec(
      skip: 0,
      size: 12,
      search: default,
	  filterFavorites: false,
	  memberId: 0);

    var result = sut.Evaluate(_archiveVideos);

    result.Count().ShouldBe(_archiveVideos.Count());
  }

  [Theory]
  [InlineData("markdown", 1)]
  [InlineData("some nonpresent value", 0)]
  public void ReturnsFilteredListGivenSearchExpression(string search, int expectedCount)
  {
    var sut = new ArchiveVideoByPageSpec(
      skip: 0,
      size: 12,
      search: search,
      filterFavorites: false,
	  memberId: 0);

    var result = sut.Evaluate(_archiveVideos);

    result.Count().ShouldBe(expectedCount);
  }

  [Theory]
  [InlineData(0, 2)]
  [InlineData(1, 1)]
  [InlineData(2, 0)]
  [InlineData(3, 0)]
  public void ReturnsFilteredListGivenSkipExpression(int skip, int expectedCount)
  {
    var sut = new ArchiveVideoByPageSpec(
      skip: skip,
      size: 12,
      search: default,
	  filterFavorites: false,
	  memberId: 0);

    var result = sut.Evaluate(_archiveVideos);

    result.Count().ShouldBe(expectedCount);
  }

  [Theory]
  [InlineData(0, 0)]
  [InlineData(1, 1)]
  [InlineData(2, 2)]
  [InlineData(3, 2)]
  public void ReturnsFilteredListGivenSizeExpression(int size, int expectedCount)
  {
    var sut = new ArchiveVideoByPageSpec(
      skip: 0,
      size: size,
      search: default,
	  filterFavorites: false,
	  memberId: 0);

    var result = sut.Evaluate(_archiveVideos);

    result.Count().ShouldBe(expectedCount);
  }
}
