using System.Linq;
using DevBetterWeb.Core.Entities;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberAddFavoriteArchiveVideo
{
  private readonly int _validArchiveVideoId = 456;

  [Fact]
  public void ShouldAddFavoriteArchiveVideoGivenArchiveVideo()
  {
    var member = MemberHelpers.CreateWithInternalConstructor();

    var archiveVideo = new ArchiveVideo
    {
      Id = _validArchiveVideoId,
    };

    member.AddFavoriteArchiveVideo(archiveVideo);

	  member.FavoriteArchiveVideos.Single().ArchiveVideoId.ShouldBe(_validArchiveVideoId);
	  member.FavoriteArchiveVideos.Single().MemberId.ShouldBe(member.Id);
  }

  [Fact]
  public void ShouldDoNothingGivenDuplicateArchiveVideo()
  {
    var member = MemberHelpers.CreateWithInternalConstructor();

    var archiveVideo = new ArchiveVideo
    {
      Id = _validArchiveVideoId,
    };

    member.AddFavoriteArchiveVideo(archiveVideo);
    member.AddFavoriteArchiveVideo(archiveVideo);

    member.FavoriteArchiveVideos.Count().ShouldBe(1);
  }
}
