using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Helper;
using DevBetterWeb.Vimeo.Tests.Builders;
using DevBetterWeb.Vimeo.Tests.Extensions;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests;

public class VttConvertTest
{

  [Fact]
  public async Task ReturnsVttGivenSrtTest()
  {
    var srt = await TestFileHelper.BuildSrtAsync();
    var expectedVtt = await TestFileHelper.BuildVttAsync();

    var vtt = SubtitleConverter.ConvertSrtToVtt(srt);

    vtt.RemoveSpecialCharacters().ShouldBe(expectedVtt.RemoveSpecialCharacters());
  }
}
