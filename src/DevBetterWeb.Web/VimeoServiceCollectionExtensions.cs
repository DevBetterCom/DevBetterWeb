using Microsoft.Extensions.DependencyInjection;
using NimblePros.Vimeo.Constants;
using NimblePros.Vimeo.Extensions;

namespace DevBetterWeb.Web;

public static class VimeoServiceCollectionExtensions
{
	private static readonly string[] VimeoHttpClientNames =
	[
		ServiceConstants.VIMEO_API_HTTP_NAME,
		ServiceConstants.VIMEO_API_TUS_HTTP_NAME,
		ServiceConstants.VIMEO_OEMBED_HTTP_NAME,
	];

	/// <summary>
	/// Registers NimblePros.Vimeo, then removes the standard resilience handler that
	/// AddServiceDefaults adds to every HttpClient. Its timeouts (10s per attempt, 30s total)
	/// would override the Vimeo clients' 60-minute timeout and cut off large chunk uploads.
	/// The clients must be configured after AddVimeoServices, which throws if they were
	/// configured earlier.
	/// </summary>
	public static IServiceCollection AddDevBetterVimeoServices(this IServiceCollection services, string token)
	{
		services.AddVimeoServices(token);

		foreach (var clientName in VimeoHttpClientNames)
		{
			// RemoveAllResilienceHandlers is the documented way to opt a client out of
			// ConfigureHttpClientDefaults resilience, but it is still marked experimental.
#pragma warning disable EXTEXP0001
			services.AddHttpClient(clientName).RemoveAllResilienceHandlers();
#pragma warning restore EXTEXP0001
		}

		return services;
	}
}
