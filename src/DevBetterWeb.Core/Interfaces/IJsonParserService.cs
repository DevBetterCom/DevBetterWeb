using System.Text.Json;

namespace DevBetterWeb.Core.Interfaces;

public interface IJsonParserService
{
	JsonDocument Parse(string json);
}
