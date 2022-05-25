using DevBetterWeb.Core.ValueObjects;
using FluentAssertions;
using FluentAssertions.Execution;
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

    using (new AssertionScope())
    {
      memberFavoriteArchiveVideo.ArchiveVideoId.Should().Be(_validArchiveVideoId);
      memberFavoriteArchiveVideo.MemberId.Should().Be(_validMemberId);
    }
  }
}
