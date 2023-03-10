using DevBetterWeb.Infrastructure.DiscordWebhooks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;

namespace DevBetterWeb.Infrastructure.Services;

public class DiscordWebhookService : IDiscordWebhookService
{
	private readonly HttpClient _httpClient = new();
	private readonly IAppLogger<DiscordWebhookService> _logger;

	public DiscordWebhookService(IAppLogger<DiscordWebhookService> logger)
	{
		_logger = logger;
	}

	public Task<HttpResponseMessage> SendAsync(ulong id, string token, string contentBody, string? username = null, string? avatarUrl = null, bool isTTS = false, IEnumerable<Embed>? embeds = null)
	{
		var webhookUrl = $"https://discordapp.com/api/webhooks/{id}/{token}";

		return SendAsync(webhookUrl, contentBody, username, avatarUrl, isTTS, embeds);
	}

	public async Task<HttpResponseMessage> SendAsync(string webhookUrl, string contentBody, string? username = null, string? avatarUrl = null, bool isTTS = false, IEnumerable<Embed>? embeds = null)
	{
		var request = new WebHookRequest(contentBody, username, avatarUrl, isTTS, embeds);
		var json = JsonSerializer.Serialize(request);

		using var content = new StringContent(json, Encoding.UTF8, "application/json");
		try
		{
			return await _httpClient.PostAsync(webhookUrl, content);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending webhook", webhookUrl, content);
			return new HttpResponseMessage();
		}
	}
}
