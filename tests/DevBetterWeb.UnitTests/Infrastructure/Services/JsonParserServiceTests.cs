using System.Text.Json;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;

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
		string invalidJson = JsonSerializer.Serialize(new { Name = "Test" }).TrimEnd('}');

		Assert.ThrowsAny<JsonException>(() => this._jsonParserService.Parse(invalidJson));
	}

	[Fact]
	public void Parse_ValidJson_WorksAsExpected()
	{
		string validJson = JsonSerializer.Serialize(new { Name = "Test" });
		var expectedResult = JsonDocument.Parse(validJson);

		var actualResult = this._jsonParserService.Parse(validJson);

		expectedResult.Should().BeEquivalentTo(actualResult, opt => opt.ComparingByMembers<JsonElement>());
	}

	[Fact]
	public void Parse_InvalidJson_LogsErrorMessage()
	{
		string invalidJson = JsonSerializer.Serialize(new { Name = "Test" }).TrimEnd('}');

		try
		{
			this._jsonParserService.Parse(invalidJson);
		}
		catch (JsonException ex)
		{
			// The act is throwing an error. It's intercepted, because we need to test the logging.
			Assert.NotNull(ex);
			this._logger.ReceivedLogError(1, ex, $"Failed JSON parsing for: {invalidJson}");
		}

		this._logger.ReceivedLogInformation(1, "JSON parsing has failed");
	}

	[Fact]
	public void Parse_ValidJson_LogsSuccessInfoMessage()
	{
		string validJson = JsonSerializer.Serialize(new { Name = "Test" });

		this._jsonParserService.Parse(validJson);

		this._logger.ReceivedLogInformation(1, "JSON parsing is successful");
	}
}
