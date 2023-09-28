using System.Collections.Generic;
using FluentAssertions;
using Flurl.Http.Testing;
using Moq;
using Xunit;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Services;
using NimblePros.Vimeo.Models;

namespace DevBetterWeb.UnitTests.Web.Services;

public class VideoDetailsServiceTests
{
	private readonly HttpTest _httpTest = new HttpTest();

	[Fact]
	public async void GetTranscript_Returns_Empty_String_When_No_TextTracks()
	{
		List<TextTrack> textTracks = new();
		var vttServiceMock = new Mock<IWebVTTParsingService>();
		vttServiceMock.Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns("test");
		var videoDetailsService = new VideoDetailsService(null!, null!, null!, null!, vttServiceMock.Object);

		var result = await videoDetailsService.GetTranscriptAsync(textTracks, "https://it-does-not-matter-for-this-test.com");

		result.Should().BeEmpty();
	}

	[Fact]
	public async void GetTranscript_Returns_Empty_String_When_TextTrackLink_Is_Invalid()
	{
		List<TextTrack> textTracks = new() { new TextTrack { Link = "I am most definitely not a valid url" } };
		_httpTest.RespondWith("Me no findy", 404);
		var vttServiceMock = new Mock<IWebVTTParsingService>();
		vttServiceMock.Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns("test");
		var videoDetailsService = new VideoDetailsService(null!, null!, null!, null!, vttServiceMock.Object);

		var result = await videoDetailsService.GetTranscriptAsync(textTracks, "https://it-does-not-matter-for-this-test.com");

		result.Should().BeEmpty();
	}
}
