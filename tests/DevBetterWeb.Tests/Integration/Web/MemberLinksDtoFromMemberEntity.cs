using DevBetterWeb.Web.Models;
using Xunit;

namespace DevBetterWeb.Tests.Web;

public class MemberLinksDtoFromMemberEntity
{
  [Fact]
  public void ReturnsInputYouTubeUrlIfContainsQuestionMark()
  {
    var member = MemberHelpers.CreateWithDefaultConstructor();

    string youtubeInput = "https://www.youtube.com/ardalis?";

    member.UpdateLinks(null, null, null, null, null, null, youtubeInput, null, null, null);

    MemberLinksDTO dto = MemberLinksDTO.FromMemberEntity(member);

    var result = dto.YouTubeUrl;

    Assert.Equal(youtubeInput, result);
  }

  [Fact]
  public void ReturnsAlteredYouTubeUrlIfContainsNoQuestionMark()
  {
    var member = MemberHelpers.CreateWithDefaultConstructor();

    string youtubeInput = "https://www.youtube.com/ardalis";

    member.UpdateLinks(null, null, null, null, null, null, youtubeInput, null, null, null);

    MemberLinksDTO dto = MemberLinksDTO.FromMemberEntity(member);

    var result = dto.YouTubeUrl;

    Assert.Equal(youtubeInput + "?sub_confirmation=1", result);
  }

}
