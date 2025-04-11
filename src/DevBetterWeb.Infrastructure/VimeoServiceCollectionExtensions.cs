using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using NimblePros.ApiClient.Models;
using NimblePros.Vimeo.Interfaces;
using NimblePros.Vimeo.Services.UtilityServices;
using NimblePros.Vimeo.VideoTusService;

namespace DevBetterWeb.Infrastructure;

public static class VimeoServiceCollectionExtensions
{
	public static IServiceCollection RegisterVimeoServicesDependencies(this IServiceCollection services, string token)
	{
		Action<HttpClient> configureVimeoApi = client =>
		{
			client.BaseAddress = new Uri("https://api.vimeo.com");
			client.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json; version=3.4, application/json");
			client.Timeout = TimeSpan.FromMinutes(60);
		};

		Action<HttpClient> configureVimeoOEmbed = client =>
		{
			client.BaseAddress = new Uri("https://vimeo.com");
			client.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json; version=3.4, application/json");
			client.Timeout = TimeSpan.FromMinutes(60);
		};

		Action<HttpClient> configureVimeoTus = client =>
		{
			client.BaseAddress = new Uri("https://api.vimeo.com");
			client.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json; version=3.4, application/json");
			client.DefaultRequestHeaders.Add("Tus-Resumable", "1.0.0");
			client.Timeout = TimeSpan.FromMinutes(60);
		};

		// Register named HttpClients for injection
		services.AddHttpClient("VimeoApi", configureVimeoApi);
		services.AddHttpClient("VimeoOEmbed", configureVimeoOEmbed);
		services.AddHttpClient("VimeoTusApi", configureVimeoTus);

		// Register ClientApiConfigurations (used by NimblePros.Vimeo)
		var clientConfigs = new ClientApiConfigurations()
			.AddHttpClientConfiguration(new HttpClientConfiguration("VimeoApi", configureVimeoApi, token))
			.AddHttpClientConfiguration(new HttpClientConfiguration("VimeoTusApi", configureVimeoTus, token))
			.AddHttpClientConfiguration(new HttpClientConfiguration("VimeoOEmbed", configureVimeoOEmbed));

		services.AddSingleton(clientConfigs);

		// Core Vimeo services
		services.AddScoped<ISleepService, SleepService>();
		services.AddScoped<IUploadVideoSessionService, UploadVideoSessionService>();
		services.AddScoped<IUploadVideoTusService, UploadVideoTusService>();

		return services;
	}

}

public class AuthHeaderHandler : DelegatingHandler
{
	private readonly string _token;

	public AuthHeaderHandler(string token)
	{
		_token = token;
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
		return base.SendAsync(request, cancellationToken);
	}
}
