using System.Linq;
using DevBetterWeb.Core.Entities;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberRemoveFavoriteArchiveVideo
{
  private readonly int _validArchiveVideoId = 456;

  [Fact]
  public void ShouldDoNothingGivenFavoriteArchiveVideoNotInFavoriteArchiveVideos()
  {
    var member = MemberHelpers.CreateWithInternalConstructor();
	var existingArchiveVideo = new ArchiveVideo
    {
      Id = _validArchiveVideoId,
    };
    member.AddFavoriteArchiveVideo(existingArchiveVideo);
    int expectedCount = member.FavoriteArchiveVideos.Count();

    var nonexistingArchiveVideo = new ArchiveVideo
    {
      Id = _validArchiveVideoId + 1,
    };
    member.RemoveFavoriteArchiveVideo(nonexistingArchiveVideo);

    member.FavoriteArchiveVideos.Count().ShouldBe(expectedCount);
  }

  [Fact]
  public void ShouldRemoveFavoriteArchiveVideoGivenExistingFavoriteArchiveVideo()
  {
    var member = MemberHelpers.CreateWithInternalConstructor();
	var archiveVideo = new ArchiveVideo
    {
      Id = _validArchiveVideoId,
    };
    member.AddFavoriteArchiveVideo(archiveVideo);

		member.RemoveFavoriteArchiveVideo(archiveVideo);
    
    member.FavoriteArchiveVideos.ShouldBeEmpty();
  }
}
