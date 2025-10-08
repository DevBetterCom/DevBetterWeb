using DevBetterWeb.Core.ValueObjects;
using Shouldly;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.ValueObjects;

public class MemberFavoriteArchiveVideoConstructor
{
  private readonly int _validMemberId = 123;
  private readonly int _validArchiveVideoId = 456;

  [Fact]
  public void SetValuesGivenMemberIdAndArchiveVideoId()
  {
    var memberFavoriteArchiveVideo = new MemberFavoriteArchiveVideo(_validMemberId, _validArchiveVideoId);

    memberFavoriteArchiveVideo.ArchiveVideoId.ShouldBe(_validArchiveVideoId);
    memberFavoriteArchiveVideo.MemberId.ShouldBe(_validMemberId);
  }
}
