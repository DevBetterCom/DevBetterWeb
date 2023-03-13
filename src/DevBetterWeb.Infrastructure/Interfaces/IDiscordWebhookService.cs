using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.DiscordWebhooks;

namespace DevBetterWeb.Infrastructure.Interfaces;

public interface IDiscordWebhookService
{
	Task<HttpResponseMessage> SendAsync(ulong id, string token, string contentBody, string? username = null, string? avatarUrl = null, bool isTTS = false, IEnumerable<Embed>? embeds = null);
	Task<HttpResponseMessage> SendAsync(string webhookUrl, string contentBody, string? username = null, string? avatarUrl = null, bool isTTS = false, IEnumerable<Embed>? embeds = null);
}
