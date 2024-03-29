using Xunit;
using FluentAssertions;
using DevBetterWeb.Web.Services;

namespace DevBetterWeb.UnitTests.Web.Services;

public class WebVTTParsingServiceTests
{
	[Fact]
	public void Parse_WebVTT()
	{
		var vtt = @"WEBVTT - This file was automatically generated by VIMEO

0
00:00:06.200 --> 00:00:10.100
All right. Hey everybody. It is Friday, September

1
00:00:09.100 --> 00:00:11.700
 2nd. Let's

2
00:00:13.300 --> 00:00:15.200
turn off that and

3
00:00:16.300 --> 00:00:17.300
get people invited.

";

		var expected = @"<a href=""https://devbetter.com/Videos/Details/1234/6"">All right. Hey everybody. It is Friday, September</a>
<a href=""https://devbetter.com/Videos/Details/1234/9"">2nd. Let's</a>
<a href=""https://devbetter.com/Videos/Details/1234/13"">turn off that and</a>
<a href=""https://devbetter.com/Videos/Details/1234/16"">get people invited.</a>
";

		var actual = new WebVTTParsingService().Parse(vtt, "https://devbetter.com/Videos/Details/1234");

		actual.Should().Be(expected);
	}
}
