using DevBetterWeb.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace DevBetterWeb.Infrastructure.Extensions;
public static class ConfigurationExtensions
{
	public static EmailSettings GetEmailSettings(this IConfiguration configuration)
	{
		return GetSection<EmailSettings>(configuration, "EmailSettings");
	}

	public static T GetSection<T>(this IConfiguration configuration, string key) where T : new()
	{
		var section = configuration.GetSection(key);
		var data = new T();
		section.Bind(data);

		return data;
	}
}
