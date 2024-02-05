using System.Text.Json;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace DevBetterWeb.UnitTests.Infrastructure.Services;
public class JsonParserServiceTests
{
	private readonly IJsonParserService _jsonParserService;
	private readonly ILogger<JsonParserService> _logger = Substitute.For<ILogger<JsonParserService>>();

	public JsonParserServiceTests()
	{
		this._jsonParserService = new JsonParserService(this._logger);
	}

	[Fact]
	public void Parse_InvalidJson_ThrowsJsonException()
	{
		string invlaidJson = JsonSerializer.Serialize(new { Name = "Test" }).TrimEnd('}');

		Assert.ThrowsAny<JsonException>(() => this._jsonParserService.Parse(invlaidJson));
	}

	[Fact]
	public void Parse_ValidJson_WorksAsExpected()
	{
		string vlaidJson = JsonSerializer.Serialize(new { Name = "Test" });
		var expectedResult = JsonDocument.Parse(vlaidJson);

		var actualResult = this._jsonParserService.Parse(vlaidJson);

		expectedResult.Should().BeEquivalentTo(actualResult, opt => opt.ComparingByMembers<JsonElement>());
	}

	[Fact]
	public void Parse_InvalidJson_LogsErrorMessage()
	{
		string invlaidJson = JsonSerializer.Serialize(new { Name = "Test" }).TrimEnd('}');

		try
		{
			var actualResult = this._jsonParserService.Parse(invlaidJson);
		}
		catch (JsonException ex)
		{
			// The act is throwing an error. It's intercepted, because we need to test the logging.
			Assert.NotNull(ex);
		}

		this._logger.Received().LogError(Arg.Any<JsonException>() ,$"Failed JSON parsing for: {invlaidJson}");
		// this._logger.VerifyLog(l => l.LogInformation("JSON parsing has failed"));
		this._logger.Received().LogInformation("JSON parsing has failed");
	}

	[Fact]
	public void Parse_ValidJson_LogsSuccessInfoMessage()
	{
		string vlaidJson = JsonSerializer.Serialize(new { Name = "Test" });
		var expectedResult = JsonDocument.Parse(vlaidJson);

		var actualResult = this._jsonParserService.Parse(vlaidJson);

		// this._logger.VerifyLog(l => l.LogInformation("JSON parsing is successful"));
		this._logger.Received().LogInformation("JSON parsing is successful");

	}
}
