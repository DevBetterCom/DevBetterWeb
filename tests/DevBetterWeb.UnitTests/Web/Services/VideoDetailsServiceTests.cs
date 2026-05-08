using System.Collections.Generic;
using Flurl.Http.Testing;
using NSubstitute;
using Xunit;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Services;
using NimblePros.Vimeo.Models;
using System.Threading.Tasks;
using Shouldly;

namespace DevBetterWeb.UnitTests.Web.Services;

public class VideoDetailsServiceTests
{
	private readonly HttpTest _httpTest = new HttpTest();

	[Fact]
	public async Task GetTranscript_Returns_Empty_String_When_No_TextTracks()
	{
		List<TextTrack> textTracks = new();
		var vttServiceMock = Substitute.For<IWebVTTParsingService>();
		vttServiceMock.Parse(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns("test");
		var videoDetailsService = new VideoDetailsService(null!, null!, null!, null!, vttServiceMock);

		var result = await videoDetailsService.GetTranscriptAsync(textTracks, "https://it-does-not-matter-for-this-test.com");

		result.ShouldBeEmpty();
	}

	[Fact]
	public async Task GetTranscript_Returns_Empty_String_When_TextTrackLink_Is_Invalid()
	{
		List<TextTrack> textTracks = new() { new TextTrack { Link = "I am most definitely not a valid url" } };
		_httpTest.RespondWith("Me no findy", 404);
		var vttServiceMock = Substitute.For<IWebVTTParsingService>();
		vttServiceMock.Parse(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns("test");
		var videoDetailsService = new VideoDetailsService(null!, null!, null!, null!, vttServiceMock);

		var result = await videoDetailsService.GetTranscriptAsync(textTracks, "https://it-does-not-matter-for-this-test.com");

		result.ShouldBeEmpty();
	}
}
