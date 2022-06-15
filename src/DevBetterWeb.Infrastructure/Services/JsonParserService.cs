using System.Text.Json;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Infrastructure.Services;

public class JsonParserService : IJsonParserService
{
	private readonly ILogger<JsonParserService> _logger;

	public JsonParserService(ILogger<JsonParserService> logger)
	{
		_logger = logger;
	}

	public JsonDocument Parse(string json)
	{
		bool isParseSuccessful = false;
		_logger.LogInformation($"JSON parsing in progress: {json}");

		try
		{
			var result = JsonDocument.Parse(json);
			isParseSuccessful = true;

			return result;
		}
		catch (JsonException jsonException)
		{
			_logger.LogError(jsonException, $"Failed JSON parsing for: {json}");
			throw;
		}
		finally
		{
			_logger.LogInformation($"JSON parsing {(isParseSuccessful ? "is successful" : "has failed")}");
		}
	}
}
