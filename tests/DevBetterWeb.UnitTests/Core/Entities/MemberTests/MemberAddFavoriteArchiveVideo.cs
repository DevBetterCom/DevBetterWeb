using System.Linq;
using DevBetterWeb.Core.Entities;
using FluentAssertions;
using FluentAssertions.Execution;
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

    using (new AssertionScope())
    {
	  member.FavoriteArchiveVideos.Single().ArchiveVideoId.Should().Be(_validArchiveVideoId);
	  member.FavoriteArchiveVideos.Single().MemberId.Should().Be(member.Id);
    }
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

    member.FavoriteArchiveVideos.Count.Should().Be(1);
  }
}
